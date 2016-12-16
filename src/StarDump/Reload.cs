using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Starcounter;
using Starcounter.Database.Interop;

namespace StarDump
{
    using BluestarColumn = Starcounter.Database.Interop.scdbmetalayer.STAR_COLUMN_DEFINITION_NAMES;


    public class Reload
    {
        public Configuration Configuration { get; protected set; }
        public SqlHelper SqlHelper { get; protected set; }
        public event EventHandler<string> ReloadTableStart;
        public event EventHandler<string> ReloadTableFinish;

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
            cn.Open();
            
            this.SqlHelper.SetupSqliteConnection(cn);
            this.CreateTablesAndInsertData(cn, out tablesCount, out rowsCount);

            cn.Close();
            host.Dispose();
            watch.Stop();

            RunResult result = new RunResult(watch.Elapsed, tablesCount, rowsCount);

            return result;
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
            List<ReloadTable> roots = new List<ReloadTable>();

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

        protected void CreateTables(SqliteConnection cn, List<ReloadTable> tables)
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

                this.InsertTableData(cn, table, table.Columns, out tableRowsCount);
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
                this.CreateTable(table, table.Columns);
            });
        }

        protected void CreateTable(ReloadTable table, List<ReloadColumn> columns)
        {
            List<BluestarColumn> bluestarColumns = columns.Where(x => !x.Inherited).Select(x => this.GetBluestarColumn(x)).ToList();

            ushort layout;
            string parentName = table.ParentName;
            ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;

            bluestarColumns.Add(new BluestarColumn() { name = null });
            Starcounter.Db.MetalayerCheck(scdbmetalayer.star_create_table_by_names(dbHandle, table.Name, parentName, bluestarColumns.ToArray(), out layout));
        }

        protected void InsertTableData(SqliteConnection cn, ReloadTable table, List<ReloadColumn> columns, out ulong rowsCount)
        {
            this.ReloadTableStart?.Invoke(this, table.Name);

            string sql = this.SqlHelper.GenerateSelectFrom(table.Name, columns.Select(x => x.Name).ToArray());
            SqliteCommand cmd = new SqliteCommand(sql, cn);
            SqliteDataReader reader = cmd.ExecuteReader();
            rowsCount = 0;

            while (reader.Read())
            {
                long id = reader.GetInt64(0);
                UnloadRow row = new UnloadRow((ulong)id, 0);

                for (int i = 0; i < columns.Count; i++)
                {
                    ReloadColumn c = columns[i];
                    object value = reader.GetValue(i + 1);

                    row[c.Name] = this.SqlHelper.ConvertFromSqliteToStarcounter(c.DataType, value);
                }

                Db.Transact(() =>
                {
                    row.Insert(table.Name, columns.ToArray());
                });

                rowsCount++;
            }

            reader.Dispose();

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
                long parentId = reader.GetInt64(2);
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