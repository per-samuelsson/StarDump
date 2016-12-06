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
        public event EventHandler<Starcounter.Metadata.RawView> UnloadTableStart;
        public event EventHandler<Starcounter.Metadata.RawView> UnloadTableFinish;

        public Unload(Configuration config)
        {
            this.Configuration = config;
        }

        public RunResult Run()
        {
            Stopwatch watch = new Stopwatch();
            Configuration config = this.Configuration;
            FileInfo fi = config.FileInfo;
            int tablesCount = 0;
            ulong rowsCount = 0;

            watch.Start();

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            else if (fi.Exists)
            {
                fi.Delete();
            }

            SqlHelper helper = new SqlHelper();
            string connectionString = string.Format("Data Source={0}", fi.FullName);
            SqliteConnection cn = new SqliteConnection(connectionString);

            ResultRow.RegisterDatabaseType();
            cn.Open();

            string sql = helper.GenerateCreateMetadataTables();
            this.ExecuteNonQuery(sql, cn);

            string[] args = new string[] { config.DatabaseName };
            var host = new AppHostBuilder().AddCommandLine(args).Build();
            host.Start();

            Db.Transact(() =>
            {
                ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;
                Starcounter.Metadata.RawView[] tables = this.SelectTables();
                tablesCount = tables.Length;

                foreach (Starcounter.Metadata.RawView t in tables)
                {
                    Starcounter.Internal.Metadata.SetSpecifier specifier = DbCrud.GetSetSpecifier(t, dbHandle);

                    this.UnloadTableStart?.Invoke(this, t);

                    Starcounter.Metadata.Column[] columns = this.SelectTableColumns(t.FullName);

                    sql = helper.GenerateInsertMetadataTable(t);
                    this.ExecuteNonQuery(sql, cn);

                    sql = helper.GenerateInsertMetadataColumns(t, columns);
                    this.ExecuteNonQuery(sql, cn);

                    sql = helper.GenerateCreateTable(t, columns);
                    this.ExecuteNonQuery(sql, cn);

                    string query = "SELECT * FROM \"" + t.FullName + "\"";
                    var rows = Db.SQL<ResultRow>(query);
                    List<ResultRow> temp = new List<ResultRow>();

                    foreach (var r in rows)
                    {
                        var m = Db.FromId<Starcounter.Internal.Metadata.MotherOfAllLayouts>(r.DbGetIdentity());
                        var proxy = m as Starcounter.Abstractions.Database.IDbProxy;
                        string s = Db.GetSetSpecifier(proxy, dbHandle);

                        if (specifier.TypeId != s)
                        {
                            continue;
                        }

                        rowsCount++;
                        r.Fill(t.FullName, columns);

                        if (temp.Count < config.InsertRowsBufferSize)
                        {
                            temp.Add(r);
                            continue;
                        }

                        sql = helper.GenerateInsertInto(t.FullName, columns, temp.ToArray());
                        this.ExecuteNonQuery(sql, cn);
                        temp.Clear();
                    }

                    if (temp.Any())
                    {
                        sql = helper.GenerateInsertInto(t.FullName, columns, temp.ToArray());
                        this.ExecuteNonQuery(sql, cn);
                        temp.Clear();
                    }

                    this.UnloadTableFinish?.Invoke(this, t);
                }
            });

            cn.Close();
            watch.Stop();

            RunResult result = new RunResult(watch.Elapsed, tablesCount, rowsCount);

            return result;
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

        protected Starcounter.Metadata.Column[] SelectTableColumns(string tableName)
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

            return columns.ToArray();
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