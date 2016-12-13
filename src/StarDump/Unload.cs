using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Starcounter;

namespace StarDump
{
    public class Unload
    {
        public Configuration Configuration { get; protected set; }
        public SqlHelper SqlHelper { get; protected set; }
        public event EventHandler<string> RowsChunkUnloaded;

        public Unload(Configuration config)
        {
            this.Configuration = config;
            this.SqlHelper = new SqlHelper();
        }

        public RunResult Run()
        {
            // System.Threading.ThreadPool.SetMinThreads(50, 50);
            // System.Threading.ThreadPool.SetMaxThreads(10000, 10000);
            // System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            // ComPlus_ThreadPool_ForceMinWorkerThreads=50
            // ComPlus_ThreadPool_ForceMaxWorkerThreads=10000
            // https://github.com/dotnet/corefx/issues/5920

            Stopwatch watch = new Stopwatch();
            Configuration config = this.Configuration;
            int tablesCount = 0;
            ulong rowsCount = 0;

            watch.Start();

            SqliteConnection cn = this.GetConnection();
            string[] args = new string[] { config.DatabaseName };
            var host = new AppHostBuilder().AddCommandLine(args).Build();
            
            host.Start();
            UnloadRow.RegisterDatabaseType();
            cn.Open();
            this.SqlHelper.SetupSqliteConnection(cn);

            string sql = this.SqlHelper.GenerateCreateMetadataTables();
            this.SqlHelper.ExecuteNonQuery(sql, cn);
            
            List<Task> tasks = new List<Task>();

            Db.Transact(() =>
            {
                ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;
                Starcounter.Metadata.RawView[] tables = this.SelectTables();
                tablesCount = tables.Length;

                Dictionary<string, Starcounter.Metadata.RawView> tablesDictionary = new Dictionary<string, Starcounter.Metadata.RawView>();
                Dictionary<string, UnloadColumn[]> columnsDictionary = new Dictionary<string, UnloadColumn[]>();
                Dictionary<string, List<UnloadRow>> rowsDictionary = new Dictionary<string, List<UnloadRow>>();
                Dictionary<string, ulong> rowsCountDictionary = new Dictionary<string, ulong>();

                foreach (Starcounter.Metadata.RawView t in tables)
                {
                    string specifier = this.GetSetSpecifier(dbHandle, t);
                    UnloadColumn[] columns = this.SelectTableColumns(t.FullName);

                    tablesDictionary.Add(specifier, t);
                    columnsDictionary.Add(specifier, columns);
                    rowsDictionary.Add(specifier, new List<UnloadRow>());
                    rowsCountDictionary.Add(specifier, 0);

                    this.CreateTableAndInsertMetadata(cn, t, columns);
                }

                this.SqlHelper.ExecuteNonQuery("BEGIN TRANSACTION", cn);

                string query = "SELECT m FROM Starcounter.Internal.Metadata.MotherOfAllLayouts m";
                var rows = Db.SQL<Starcounter.Internal.Metadata.MotherOfAllLayouts>(query);

                foreach (Starcounter.Internal.Metadata.MotherOfAllLayouts row in rows)
                {
                    string specifier = this.GetSetSpecifier(dbHandle, row);

                    if (!rowsDictionary.ContainsKey(specifier))
                    {
                        continue;
                    }

                    List<UnloadRow> list = rowsDictionary[specifier];
                    var proxy = row as Starcounter.Abstractions.Database.IDbProxy;
                    UnloadRow r = new UnloadRow(proxy.DbGetIdentity(), proxy.DbGetReference());
                    Starcounter.Metadata.RawView table = tablesDictionary[specifier];
                    UnloadColumn[] columns = columnsDictionary[specifier];

                    r.Fill(table.FullName, columns);
                    list.Add(r);
                    rowsCount++;
                    rowsCountDictionary[specifier]++;

                    if (list.Count < this.Configuration.InsertRowsBufferSize)
                    {
                        continue;
                    }

                    tasks.Add(this.InsertRows(cn, table.FullName, columns, list.ToArray()));
                    list.Clear();
                    this.RowsChunkUnloaded?.Invoke(this, table.FullName);
                }

                foreach (KeyValuePair<string, List<UnloadRow>> item in rowsDictionary)
                {
                    string specifier = item.Key;
                    Starcounter.Metadata.RawView t = tablesDictionary[specifier];
                    ulong count = rowsCountDictionary[specifier];

                    sql = this.SqlHelper.GenerateUpdateMetadataTableRowsCount(t.FullName, count);
                    this.SqlHelper.ExecuteNonQuery(sql, cn);

                    if (!item.Value.Any())
                    {
                        continue;
                    }

                    List<UnloadRow> list = rowsDictionary[specifier];
                    UnloadColumn[] columns = columnsDictionary[specifier];

                    tasks.Add(this.InsertRows(cn, t.FullName, columns, list.ToArray()));
                }
            });

            Task.WaitAll(tasks.ToArray());
            this.SqlHelper.ExecuteNonQuery("COMMIT TRANSACTION", cn);

            cn.Close();
            host.Dispose();
            watch.Stop();

            RunResult result = new RunResult(watch.Elapsed, tablesCount, rowsCount);

            return result;
        }

        protected SqliteConnection GetConnection()
        {
            FileInfo fi = this.Configuration.FileInfo;

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            else if (fi.Exists)
            {
                fi.Delete();
            }

            string connectionString = string.Format("Data Source={0}", fi.FullName);
            SqliteConnection cn = new SqliteConnection(connectionString);

            return cn;
        }

        protected void CreateTableAndInsertMetadata(SqliteConnection cn, Starcounter.Metadata.RawView table, UnloadColumn[] columns)
        {
            string sql = this.SqlHelper.GenerateInsertMetadataTable(table);
            this.SqlHelper.ExecuteNonQuery(sql, cn);

            sql = this.SqlHelper.GenerateInsertMetadataColumns(table, columns);
            this.SqlHelper.ExecuteNonQuery(sql, cn);

            sql = this.SqlHelper.GenerateCreateTable(table, columns);
            this.SqlHelper.ExecuteNonQuery(sql, cn);
        }

        protected Task InsertRows(SqliteConnection cn, string tableName, UnloadColumn[] columns, UnloadRow[] rows)
        {
            return Task.Run(() =>
            {
                string sql = this.SqlHelper.GenerateInsertInto(tableName, columns, rows);
                this.SqlHelper.ExecuteNonQuery(sql, cn);
            });
        }

        protected Task InsertRowWithParams(SqliteConnection cn, string sql, UnloadColumn[] columns, UnloadRow row)
        {
            return Task.Run(() =>
            {
                SqliteCommand cmd = new SqliteCommand(sql, cn);

                cmd.Parameters.Add(new SqliteParameter() { ParameterName = "@ObjectNo", Value = row.DbGetIdentity() });

                foreach (UnloadColumn c in columns)
                {
                    SqliteParameter p = new SqliteParameter();
                    object value = row[c.Name];
                    
                    p.ParameterName = "@" + c.Name;

                    if (value == null)
                    {
                        p.Value = System.DBNull.Value;
                    }
                    else
                    {
                        p.Value = value;
                    }

                    cmd.Parameters.Add(p);
                }

                cmd.ExecuteNonQuery();
            });
        }

        protected string GetSetSpecifier(ulong dbHandle, Starcounter.Metadata.RawView table)
        {
            Starcounter.Internal.Metadata.SetSpecifier specifier = DbCrud.GetSetSpecifier(table, dbHandle);

            return specifier.TypeId;
        }

        protected string GetSetSpecifier(ulong dbHandle, UnloadRow row)
        {
            var m = Db.FromId<Starcounter.Internal.Metadata.MotherOfAllLayouts>(row.DbGetIdentity());
            string s = this.GetSetSpecifier(dbHandle, m);

            return s;
        }

        protected string GetSetSpecifier(ulong dbHandle, Starcounter.Internal.Metadata.MotherOfAllLayouts row)
        {
            var proxy = row as Starcounter.Abstractions.Database.IDbProxy;
            string s = Db.GetSetSpecifier(proxy, dbHandle);

            return s;
        }

        /// <summary>
        /// Returns true if row belongs to specifier, false otherwise
        /// </summary>
        protected bool SetSpecifierEquals(ulong dbHandle, Starcounter.Internal.Metadata.SetSpecifier specifier, UnloadRow row)
        {
            string s = this.GetSetSpecifier(dbHandle, row);

            return specifier.TypeId == s;
        }

        protected Starcounter.Metadata.RawView[] SelectTables()
        {
            IEnumerable<Starcounter.Metadata.RawView> tables = Db.SQL<Starcounter.Metadata.RawView>("SELECT t FROM \"Starcounter.Metadata.RawView\" t");
            string[] prefixes = this.Configuration.SkipTablePrefixes;

            if (prefixes.Length == 1)
            {
                string prefix = prefixes[0];
                tables = tables.Where(x => !x.Name.StartsWith(prefix));
            }
            else if (prefixes.Any())
            {
                tables = tables.Where(x => !prefixes.Any(p => x.FullName.StartsWith(p)));
            }

            return tables.ToArray();
        }

        protected UnloadColumn[] SelectTableColumns(string tableName)
        {
            IEnumerable<Starcounter.Metadata.Column> columns = Db.SQL<Starcounter.Metadata.Column>("SELECT c FROM \"Starcounter.Metadata.Column\" c");
            string[] prefixes = this.Configuration.SkipColumnPrefixes;

            if (prefixes.Length == 0)
            {
                columns = columns.Where(x => x.Table.FullName == tableName);
            }
            else if (prefixes.Length == 1)
            {
                string prefix = prefixes[0];
                columns = columns.Where(x => x.Table.FullName == tableName && !x.Name.StartsWith(prefix));
            }
            else
            {
                columns = columns.Where(x => x.Table.FullName == tableName && !prefixes.Any(p => x.Name.StartsWith(p)));
            }

            return columns.Select(x => new UnloadColumn(x)).ToArray();
        }
    }
}