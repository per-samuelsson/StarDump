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
        public event EventHandler<string> UnloadTableStart;
        public event EventHandler<string> UnloadTableFinish;

        public Unload(Configuration config)
        {
            this.Configuration = config;
            this.SqlHelper = new SqlHelper();
        }

        public RunResult Run()
        {
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

            string sql = this.SqlHelper.GenerateCreateMetadataTables();
            this.ExecuteNonQuery(sql, cn);

            Db.Transact(() =>
            {
                ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;
                Starcounter.Metadata.RawView[] tables = this.SelectTables();
                tablesCount = tables.Length;
                
                List<Task> tasks = new List<Task>();

                foreach (Starcounter.Metadata.RawView t in tables)
                {
                    this.UnloadTableStart?.Invoke(this, t.FullName);

                    Starcounter.Internal.Metadata.SetSpecifier specifier = DbCrud.GetSetSpecifier(t, dbHandle);
                    UnloadColumn[] columns = this.SelectTableColumns(t.FullName);

                    string query = "SELECT * FROM \"" + t.FullName + "\"";
                    var rows = Db.SQL<UnloadRow>(query);
                    List<UnloadRow> temp = new List<UnloadRow>();
                    ulong tableRowsCount = 0;

                    this.CreateTableAndInsertMetadata(cn, t, columns);

                    foreach (var r in rows)
                    {
                        bool equals = this.SetSpecifierEquals(dbHandle, specifier, r);

                        if (!equals)
                        {
                            continue;
                        }

                        tableRowsCount++;
                        r.Fill(t.FullName, columns);
                        temp.Add(r);

                        if (temp.Count < this.Configuration.InsertRowsBufferSize)
                        {
                            continue;
                        }

                        tasks.Add(this.InsertRows(cn, t.FullName, columns, temp.ToArray()));
                        temp.Clear();
                    }

                    if (temp.Any())
                    {
                        tasks.Add(this.InsertRows(cn, t.FullName, columns, temp.ToArray()));
                    }

                    sql = this.SqlHelper.GenerateUpdateMetadataTableRowsCount(t.FullName, tableRowsCount);
                    this.ExecuteNonQuery(sql, cn);

                    rowsCount += tableRowsCount;
                    this.UnloadTableFinish?.Invoke(this, t.FullName);
                }

                Task.WaitAll(tasks.ToArray());
            });

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
            this.ExecuteNonQuery(sql, cn);

            sql = this.SqlHelper.GenerateInsertMetadataColumns(table, columns);
            this.ExecuteNonQuery(sql, cn);

            sql = this.SqlHelper.GenerateCreateTable(table, columns);
            this.ExecuteNonQuery(sql, cn);
        }

        protected Task InsertRows(SqliteConnection cn, string tableName, UnloadColumn[] columns, UnloadRow[] rows)
        {
            return Task.Run(() =>
            {
                string sql = this.SqlHelper.GenerateInsertInto(tableName, columns, rows);
                this.ExecuteNonQuery(sql, cn);
            });
        }

        /// <summary>
        /// Returns true if row belongs to specifier, false otherwise
        /// </summary>
        protected bool SetSpecifierEquals(ulong dbHandle, Starcounter.Internal.Metadata.SetSpecifier specifier, UnloadRow row)
        {
            var m = Db.FromId<Starcounter.Internal.Metadata.MotherOfAllLayouts>(row.DbGetIdentity());
            var proxy = m as Starcounter.Abstractions.Database.IDbProxy;
            string s = Db.GetSetSpecifier(proxy, dbHandle);

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

        protected void ExecuteNonQuery(string sql, SqliteConnection cn)
        {
            SqliteCommand cmd = new SqliteCommand(sql, cn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Unable to execute SQL: " + sql, ex);
            }
        }
    }
}