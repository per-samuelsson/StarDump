using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Starcounter.Core;
using Starcounter.Core.Interop;
using Starcounter.Core.Hosting;
using StarDump.Common;

namespace StarDump.Core
{
    using BluestarColumn = Starcounter.Core.Interop.scdbmetalayer.STAR_COLUMN_DEFINITION_NAMES;

    public class Reload
    {
        public Configuration Configuration { get; protected set; }
        public SqlHelper SqlHelper { get; protected set; }
        public event EventHandler<string> ReloadTableStart;
        public event EventHandler<string> ReloadTableFinish;
        public event EventHandler<string> ErrorEvent;
        public event EventHandler<string> WarningEvent;

        public Reload(Configuration config)
        {
            this.Configuration = config;
            this.SqlHelper = new SqlHelper();
        }

        public RunResult Run()
        {
            Stopwatch watch = new Stopwatch();
            Configuration config = this.Configuration;
            FileInfo fi = config.FileInfo;
            int tablesCount = 0;
            ulong rowsCount = 0;

            if (!fi.Exists)
            {
                throw new FileNotFoundException("Dump file does not exist", fi.FullName);
            }

            watch.Start();

            string connectionString = string.Format("Data Source={0}", fi.FullName);
            SqliteConnection cn = new SqliteConnection(connectionString);

            string[] args = new string[] { config.DatabaseName };
            var host = new AppHostBuilder().AddCommandLine(args).Build();

            host.Start();

            bool abort = this.AbortReload();
            if (abort)
            {
                this.ErrorEvent?.Invoke(this, "");
                this.ErrorEvent?.Invoke(this, string.Format("Aborting: --database={0} has already been initialized with tables.\r\n  Either create a new database OR\r\n  set --forcereload and take care of object ID uniqueness and table schemas.",
                    config.DatabaseName));

                host.Dispose();
                watch.Stop();

                return null;
            }

            cn.Open();
            
            this.SqlHelper.SetupSqliteConnection(cn);
            this.CreateTablesAndInsertData(cn, out tablesCount, out rowsCount);

            cn.Close();
            host.Dispose();
            watch.Stop();

            RunResult result = new RunResult(watch.Elapsed, tablesCount, rowsCount);

            return result;
        }

        /// <summary>
        /// Returns true if Database contains tables other than the <see cref="Configuration.SkipTablePrefixes"/>.
        /// Will always retun false if <see cref="Configuration.ForceReload"/> is true in which the user has to make sure of object ID uniqueness
        /// </summary>
        /// <returns></returns>
        protected bool AbortReload()
        {
            bool abort = false;
            EventHandler<string> eventHandler;

            // Check if database is empty
            Db.Transact(() =>
            {
                List<Starcounter.Metadata.RawView> tableList = Db.SQL<Starcounter.Metadata.RawView>("SELECT t FROM \"Starcounter.Metadata.RawView\" t").Where(x => x.Updatable == true).ToList();

                if (tableList.Count() == 0)
                {
                    return;  // New database
                }

                if (this.Configuration.ForceReload)
                {
                    eventHandler = WarningEvent; // Warnings only
                    eventHandler?.Invoke(this, string.Format("Warnings ({0}), make sure to take care of object ID uniqueness and table schemas:", tableList.Count()));
                }
                else
                {
                    abort = true;
                    eventHandler = ErrorEvent; // Abort with errors
                    eventHandler?.Invoke(this, string.Format("Errors ({0}):", tableList.Count()));
                }

                // Write existing tables
                foreach (Starcounter.Metadata.RawView t in tableList)
                {
                    eventHandler?.Invoke(this, string.Format("  Database is not empty: Table {0} already exists.", t.FullName));
                }
            });

            return abort;
        }

        protected Dictionary<string, byte> typeMap = new Dictionary<string, byte>()
        {
            { "bool", sccoredb.STAR_TYPE_ULONG },
            { "byte", sccoredb.STAR_TYPE_ULONG },
            { "char", sccoredb.STAR_TYPE_ULONG },
            { "DateTime", sccoredb.STAR_TYPE_ULONG }, // would be better to store as LONG, but 2.x uses ULONG
            { "decimal", sccoredb.STAR_TYPE_DECIMAL },
            { "double", sccoredb.STAR_TYPE_DOUBLE },
            { "float", sccoredb.STAR_TYPE_FLOAT },
            { "int", sccoredb.STAR_TYPE_LONG },
            { "long", sccoredb.STAR_TYPE_LONG },
            { "sbyte", sccoredb.STAR_TYPE_LONG },
            { "short", sccoredb.STAR_TYPE_LONG },
            { "string", sccoredb.STAR_TYPE_STRING },
            { "uint", sccoredb.STAR_TYPE_ULONG },
            { "ulong", sccoredb.STAR_TYPE_ULONG },
            { "ushort", sccoredb.STAR_TYPE_ULONG },
            { "byte[]", sccoredb.STAR_TYPE_BINARY },
            { "object", sccoredb.STAR_TYPE_REFERENCE }
        };

        protected void CreateTablesAndInsertData(SqliteConnection cn, out int tablesCount, out ulong rowsCount)
        {
            List<ReloadTable> tables = this.SelectTables(cn);

            rowsCount = 0;
            tablesCount = tables.Count;

            List<Task> tasks = new List<Task>();
            List<StarDump.Common.ReloadTable> roots = new List<StarDump.Common.ReloadTable>();

            foreach (ReloadTable t in tables)
            {
                ReloadTable parent = tables.FirstOrDefault(x => x.Id == t.ParentId);

                if (parent != null)
                {
                    t.ParentName = parent.Name;
                    parent.Children.Add(t);
                }
                else
                {
                    roots.Add(t);
                }
            }

            this.CreateTables(cn, roots);

            foreach (ReloadTable t in roots)
            {
                tasks.Add(this.InsertTableData(cn, tables, t));
            }

            Task.WaitAll(tasks.ToArray());
            
            foreach (ReloadTable t in tables)
            {
                rowsCount += t.RowsCount;
            }
        }

        protected void CreateTables(SqliteConnection cn, List<StarDump.Common.ReloadTable> tables)
        {
            foreach (ReloadTable t in tables)
            {
                this.CreateTable(cn, t);
                this.CreateTables(cn, t.Children);
            }
        }

        protected Task InsertTableData(SqliteConnection cn, List<ReloadTable> tables, ReloadTable table)
        {
            return Task.Run(() =>
            {
                ReloadTable parent = tables.FirstOrDefault(x => x.Id == table.ParentId);
                ulong tableRowsCount;

                this.InsertTableData(cn, table, out tableRowsCount);
                table.RowsCount = tableRowsCount;

                List<Task> tasks = new List<Task>();

                foreach (ReloadTable t in table.Children)
                {
                    tasks.Add(this.InsertTableData(cn, tables, t));
                }

                Task.WaitAll(tasks.ToArray());
            });
        }

        protected void CreateTable(SqliteConnection cn, ReloadTable table)
        {
            Db.Transact(() => 
            {
                ulong dbHandle = Starcounter.Core.Database.Transaction.Current.DatabaseContext.Handle;
                
                this.CreateTable(table, table.Columns);
                table.CrudHelper = new CrudHelper(dbHandle, table);
            });
        }

        protected void CreateTable(ReloadTable table, List<ReloadColumn> columns)
        {
            List<BluestarColumn> bluestarColumns = columns.Where(x => !x.Inherited).Select(x => this.GetBluestarColumn(x)).ToList();

            ushort layout;
            string parentName = table.ParentName;
            ulong dbHandle = Starcounter.Core.Database.Transaction.Current.DatabaseContext.Handle;

            bluestarColumns.Add(new BluestarColumn() { name = null });
            Starcounter.Core.Db.MetalayerCheck(scdbmetalayer.star_create_table_by_names(dbHandle, table.Name, parentName, bluestarColumns.ToArray(), out layout));
        }

        protected void InsertTableData(SqliteConnection cn, ReloadTable table, out ulong rowsCount)
        {
            this.ReloadTableStart?.Invoke(this, table.Name);

            string sql = this.SqlHelper.GenerateSelectFrom(table.Name, table.Columns.Select(x => x.Name).ToArray());
            SqliteCommand cmd = new SqliteCommand(sql, cn);
            SqliteDataReader reader = cmd.ExecuteReader();
            List<Task> tasks = new List<Task>();
            List<UnloadRow> temp = new List<UnloadRow>();
            rowsCount = 0;

            while (reader.Read())
            {
                long id = reader.GetInt64(0);
                UnloadRow row = new UnloadRow((ulong)id, 0);

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    ReloadColumn c = table.Columns[i];
                    object value = reader.GetValue(i + 1);

                    row[c.Name] = this.SqlHelper.ConvertFromSqliteToStarcounter(c.DataType, value);
                }

                temp.Add(row);
                rowsCount++;

                if (temp.Count < this.Configuration.InsertRowsBufferSize)
                {
                    continue;
                }

                UnloadRow[] rows = temp.ToArray();
                temp = new List<UnloadRow>();

                tasks.Add(Task.Run(() =>
                {
                    Db.Transact(() =>
                    {
                        foreach (UnloadRow r in rows)
                        {
                            r.Insert(table);
                        }
                    });
                }));
            }

            reader.Dispose();

            if (temp.Any())
            {
                tasks.Add(Task.Run(() =>
                {
                    Db.Transact(() =>
                    {
                        foreach (UnloadRow r in temp)
                        {
                            r.Insert(table);
                        }
                    });
                }));
            }

            Task.WaitAll(tasks.ToArray());

            this.ReloadTableFinish?.Invoke(this, table.Name);
        }

        protected List<ReloadTable> SelectTables(SqliteConnection cn)
        {
            List<ReloadTable> tables = new List<ReloadTable>();
            List<ReloadColumn> columns = this.SelectColumns(cn);
            SqliteCommand cmd = new SqliteCommand(this.SqlHelper.GenerateSelectTables(), cn);
            SqliteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())  
            {  
                long id = reader.GetInt64(0);
                string name = reader.GetString(1);
                long parentId = 0;

                object value = reader.GetValue(2);
                
                if (!DBNull.Value.Equals(value))
                {
                    parentId = Convert.ToInt64(value);
                }

                ReloadTable t = new ReloadTable()
                {
                    Id = id,
                    Name = name,
                    ParentId = parentId
                };

                t.Columns = columns.Where(x => x.TableId == t.Id).ToList();
                tables.Add(t);
            }

            reader.Dispose();

            return tables;
        }

        protected List<ReloadColumn> SelectColumns(SqliteConnection cn)
        {
            string sql = this.SqlHelper.GenerateSelectMetadataColumns();
            List<ReloadColumn> columns = new List<ReloadColumn>();
            SqliteCommand cmd = new SqliteCommand(sql, cn);
            SqliteDataReader reader = cmd.ExecuteReader(); 

            while (reader.Read())  
            {  
                long id = reader.GetInt64(0);
                long tableId = reader.GetInt64(1);
                string name = reader.GetString(2);
                string dataType = reader.GetString(3);
                string referenceType = reader.GetValue(4) as string;
                int nullable = reader.GetInt32(5);
                int inherited = reader.GetInt32(6);
                ReloadColumn c = new ReloadColumn()
                {
                    Id = id,
                    TableId = tableId,
                    Name = name,
                    DataType = dataType,
                    ReferenceType = referenceType,
                    Nullable = nullable == 1,
                    Inherited = inherited == 1
                };

                columns.Add(c);
            }

            reader.Dispose();

            return columns;
        }

        protected BluestarColumn GetBluestarColumn(ReloadColumn c)
        {
            return this.GetBluestarColumn(c.Name, c.DataType, c.ReferenceType, c.Nullable);
        }

        protected BluestarColumn GetBluestarColumn(string name, string dataType, string referenceType, bool nullable)
        {
            var c = new BluestarColumn();
            c.name = name;

            c.is_nullable = nullable;
            c.primitive_type = this.MapTypeToStarcounterType(dataType);

            if (c.primitive_type == sccoredb.STAR_TYPE_REFERENCE)
            {
                //c.type_name = referenceType;
                c.type_name = null;
            }

            return c;
        }

        protected byte MapTypeToStarcounterType(string name)
        {
            byte result;

            if (typeMap.TryGetValue(name, out result))
            {
                return result;
            }

            return sccoredb.STAR_TYPE_REFERENCE;
        }
    }
}