using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Starcounter.Core;
using Starcounter.Core.Hosting;
using Starcounter.Core.Database;
using StarDump.Common;

namespace StarDump.Core
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
            cn.Open();
            this.SqlHelper.SetupSqliteConnection(cn);

            string sql = this.SqlHelper.GenerateCreateMetadataTables();
            this.SqlHelper.ExecuteNonQuery(sql, cn);
            
            List<Task> tasks = new List<Task>();

            Db.Transact(() =>
            {
                ulong dbHandle = Starcounter.Core.Database.Transaction.Current.DatabaseContext.Handle;

                Dictionary<string, UnloadTable> tablesDictionary = this.SelectTables(dbHandle);

                foreach (KeyValuePair<string, UnloadTable> item in tablesDictionary)
                {
                    string specifier = item.Key;
                    UnloadTable table = item.Value;

                    table.CrudHelper = new CrudHelper(dbHandle, table);
                    this.CreateTableAndInsertMetadata(cn, table);

                    tasks.Add(Task.Run(() =>
                    {
                        StringBuilder definition = new StringBuilder();
                        this.SqlHelper.GenerateInsertIntoDefinition(table, definition);
                        table.InsertIntoDefinition = definition.ToString();
                    }));
                }
                
                tablesCount = tablesDictionary.Count;
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
                this.SqlHelper.ExecuteNonQuery("BEGIN TRANSACTION", cn);

                string query = "SELECT m FROM Starcounter.Internal.Metadata.MotherOfAllLayouts m";
                var rows = Db.SQL<Starcounter.Internal.Metadata.MotherOfAllLayouts>(query);

                foreach (Starcounter.Internal.Metadata.MotherOfAllLayouts row in rows)
                {
                    UnloadTable table;
                    string specifier = this.GetSetSpecifier(dbHandle, row);

                    if (!tablesDictionary.TryGetValue(specifier, out table))
                    {
                        continue;
                    }

                    var proxy = row as Starcounter.Core.Abstractions.Database.IDbProxy;
                    DumpRow r = new UnloadRow(proxy.DbGetIdentity(), proxy.DbGetReference());

                    table.FillRow(r);
                    table.Rows.Add(r);
                    rowsCount++;
                    table.RowsCount++;

                    if (table.Rows.Count < this.Configuration.InsertRowsBufferSize)
                    {
                        continue;
                    }

                    tasks.Add(this.InsertRows(cn, table.InsertIntoDefinition, table.Columns, table.Rows));
                    table.Rows = new List<DumpRow>();
                    this.RowsChunkUnloaded?.Invoke(this, table.FullName);
                }

                foreach (KeyValuePair<string, UnloadTable> item in tablesDictionary)
                {
                    string specifier = item.Key;
                    UnloadTable table = item.Value;

                    sql = this.SqlHelper.GenerateUpdateMetadataTableRowsCount(table.FullName, table.RowsCount);
                    this.SqlHelper.ExecuteNonQuery(sql, cn);

                    if (!table.Rows.Any())
                    {
                        continue;
                    }

                    tasks.Add(this.InsertRows(cn, table.InsertIntoDefinition, table.Columns, table.Rows));
                }
            });

            Task.WaitAll(tasks.ToArray());
            this.SqlHelper.ExecuteNonQuery("COMMIT TRANSACTION", cn);

            this.InsertMetaInfo(cn, DateTime.Now - watch.Elapsed, DateTime.Now, tablesCount, rowsCount);
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

        protected void CreateTableAndInsertMetadata(SqliteConnection cn, UnloadTable table)
        {
            string sql = this.SqlHelper.GenerateInsertMetadataTable(table);
            this.SqlHelper.ExecuteNonQuery(sql, cn);

            if (table.Columns.Any())
            {
                sql = this.SqlHelper.GenerateInsertMetadataColumns(table);
                this.SqlHelper.ExecuteNonQuery(sql, cn);
            }

            sql = this.SqlHelper.GenerateCreateTable(table);
            this.SqlHelper.ExecuteNonQuery(sql, cn);
        }

        protected Task InsertRows(SqliteConnection cn, string insertDefinition, List<StarDump.Common.UnloadColumn> columns, List<DumpRow> rows)
        {
            return Task.Run(() =>
            {
                StringBuilder sql = new StringBuilder();

                sql.Append(insertDefinition);
                this.SqlHelper.GenerateInsertIntoValues(columns, rows, sql);

                // string sql = this.SqlHelper.GenerateInsertInto(tableName, columns, rows);
                this.SqlHelper.ExecuteNonQuery(sql.ToString(), cn);
            });
        }

        protected Task InsertRowWithParams(SqliteConnection cn, string sql, UnloadColumn[] columns, DumpRow row)
        {
            return Task.Run(() =>
            {
                SqliteCommand cmd = new SqliteCommand(sql, cn);

                cmd.Parameters.Add(new SqliteParameter() { ParameterName = "@ObjectNo", Value = row.DbObjectIdentity });

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
            var proxy = table as Starcounter.Core.Abstractions.Database.IDbProxy;
            Starcounter.Internal.Metadata.SetSpecifier specifier = DbCrud.GetSetSpecifier<Starcounter.Internal.Metadata.SetSpecifier>(proxy, dbHandle);

            Debug.Assert(specifier != null);

            return specifier.TypeId;
        }

        protected string GetSetSpecifier(ulong dbHandle, DumpRow row)
        {
            var m = Db.FromId<Starcounter.Internal.Metadata.MotherOfAllLayouts>(row.DbObjectIdentity);
            string s = this.GetSetSpecifier(dbHandle, m);

            return s;
        }

        protected string GetSetSpecifier(ulong dbHandle, Starcounter.Internal.Metadata.MotherOfAllLayouts row)
        {
            var proxy = row as Starcounter.Core.Abstractions.Database.IDbProxy;

            IntPtr ptr;
            Error.Check(Starcounter.Core.Interop.sccoredb.star_context_get_setspec(dbHandle, proxy.DbGetIdentity(), proxy.DbGetReference(), out ptr));
            string specifier = DbType.BluestarPtrToStringUni(ptr);

            Debug.Assert(!string.IsNullOrEmpty(specifier));

            return specifier;
        }

        /// <summary>
        /// Returns true if row belongs to specifier, false otherwise
        /// </summary>
        protected bool SetSpecifierEquals(ulong dbHandle, Starcounter.Internal.Metadata.SetSpecifier specifier, DumpRow row)
        {
            string s = this.GetSetSpecifier(dbHandle, row);

            return specifier.TypeId == s;
        }

        protected Dictionary<string, UnloadTable> SelectTables(ulong dbHandle)
        {
            IEnumerable<Starcounter.Metadata.RawView> query = Db.SQL<Starcounter.Metadata.RawView>("SELECT t FROM \"Starcounter.Metadata.RawView\" t");
            string[] prefixes = this.Configuration.SkipTablePrefixes;
            string[] skip = this.Configuration.SkipTables;
            string[] unload = this.Configuration.UnloadTables;
            Dictionary<string, UnloadTable> tables = new Dictionary<string, UnloadTable>();

            foreach (Starcounter.Metadata.RawView t in query)
            {
                if (!t.Updatable)
                {
                    continue;
                }

                if (unload.Any() && !unload.Contains(t.FullName))
                {
                    continue;
                }

                switch (prefixes.Length)
                {
                    case 0: break;
                    case 1:
                        if (t.FullName.StartsWith(prefixes[0]))
                        {
                            continue;
                        }
                        break;
                    default:
                        if (prefixes.Any(x => t.FullName.StartsWith(x)))
                        {
                            continue;
                        }
                        break;
                }

                if (skip.Contains(t.FullName))
                {
                    continue;
                }

                string specifier = this.GetSetSpecifier(dbHandle, t);

                tables.Add(specifier, new UnloadTable(specifier, t));
            }

            this.SelectTableColumns(dbHandle, tables);

            return tables;
        }

        protected void SelectTableColumns(ulong dbHandle, Dictionary<string, UnloadTable> tables)
        {
            Dictionary<string, UnloadTable> dictionary = tables.ToDictionary(key => key.Value.FullName, val => val.Value);
            IEnumerable<Starcounter.Metadata.Column> columns = Db.SQL<Starcounter.Metadata.Column>("SELECT c FROM \"Starcounter.Metadata.Column\" c");
            string[] prefixes = this.Configuration.SkipColumnPrefixes;

            foreach (Starcounter.Metadata.Column col in columns)
            {
                string name = col.Table.FullName;

                if (!dictionary.ContainsKey(name))
                {
                    continue;
                }

                switch (prefixes.Length)
                {
                    case 0: break;
                    case 1:
                        if (col.Name.StartsWith(prefixes[0]))
                        {
                            continue;
                        }
                        break;
                    default:
                        if (prefixes.Any(x => col.Name.StartsWith(x)))
                        {
                            continue;
                        }
                        break;
                }

                dictionary[name].Columns.Add(new UnloadColumn(col, this.SqlHelper));
            }
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

            return columns.Select(x => new UnloadColumn(x, this.SqlHelper)).ToArray();
        }

        protected void InsertMetaInfo(SqliteConnection cn, DateTime unloadStart, DateTime unloadFinish, int tablesCount, ulong rowsCount)
        {
            MetaInfo info;
            List<MetaInfo> infos = new List<MetaInfo>();
            StarcounterConfiguration config = StarcounterConfiguration.GetConfiguration(this.Configuration.DatabaseName);

            info = MetaInfo.UnloadStart;
            info.Value = unloadStart.ToString("yyyy-MM-dd HH:mm:ss.fff \"GMT\"zzz");
            infos.Add(info);

            info = MetaInfo.UnloadFinish;
            info.Value = unloadFinish.ToString("yyyy-MM-dd HH:mm:ss.fff \"GMT\"zzz");
            infos.Add(info);

            info = MetaInfo.TablesCount;
            info.Value = tablesCount.ToString();
            infos.Add(info);

            info = MetaInfo.RowsCount;
            info.Value = rowsCount.ToString();
            infos.Add(info);

            info = MetaInfo.StarcounterVersion;
            info.Value = config.Version;
            infos.Add(info);

            info = MetaInfo.ApplicationName;
            info.Value = nameof(StarDump);
            infos.Add(info);

            info = MetaInfo.ApplicationVersion;
            info.Value = Assembly.Version;
            infos.Add(info);

            info = MetaInfo.StarDumpVersion;
            info.Value = Assembly.Version;
            infos.Add(info);

            info = MetaInfo.FormatVersion;
            info.Value = Assembly.FormatVersion;
            infos.Add(info);

            info = MetaInfo.DatabaseName;
            info.Value = this.Configuration.DatabaseName;
            infos.Add(info);

            info = MetaInfo.SkipTablePrefixes;
            info.Value = string.Join(", ", this.Configuration.SkipTablePrefixes);
            infos.Add(info);

            info = MetaInfo.SkipColumnPrefixes;
            info.Value = string.Join(", ", this.Configuration.SkipColumnPrefixes);
            infos.Add(info);

            string sql = this.SqlHelper.GenerateInsertMetaInfo(infos);
            this.SqlHelper.ExecuteNonQuery(sql, cn);
        }
    }
}