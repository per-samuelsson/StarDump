using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump
{
    public class CommandInterface
    {
        public static void Reload(Configuration config)
        {
            Reload reload = new Reload(config);

            reload.ReloadTableStart += (sender, table) =>
            {
                Console.WriteLine("{0}: start {1}", DateTime.Now, table);
            };

            reload.ReloadTableFinish += (sender, table) =>
            {
                Console.WriteLine("{0}: finish {1}", DateTime.Now, table);
            };

            Console.WriteLine("Reload started " + config.FileName);

            RunResult result = reload.Run();

            Console.WriteLine("Reload finished " + config.FileName);
            Console.WriteLine("Elapsed " + result.Elapsed);
            Console.WriteLine("Tables count " + result.Tables);
            Console.WriteLine("Rows count " + result.Rows);
        }

        public static void Unload(Configuration config)
        {
            Unload unload = new Unload(config);
            ulong count = 0;

            unload.RowsChunkUnloaded += (sender, table) =>
            {
                count += (ulong)config.InsertRowsBufferSize;

                if (count % 100000 == 0)
                {
                    Console.Write(".");
                }
            };

            Console.WriteLine("Unload started " + config.FileName);

            RunResult result = unload.Run();

            Console.WriteLine();
            Console.WriteLine("Unload finished " + config.FileName);
            Console.WriteLine("Elapsed " + result.Elapsed);
            Console.WriteLine("Tables count " + result.Tables);
            Console.WriteLine("Rows count " + result.Rows);
        }

        public static void SqliteMain(string[] args)
        {
            string sqliteFileRoot = "D:/Temp/Core/Database";
            string sqliteFileName = "MySQLiteDatabase.sqlite3";
            FileInfo fi = new FileInfo(Path.Combine(sqliteFileRoot, sqliteFileName));

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            string connectionString = string.Format("Data Source={0}", fi.FullName);
            SqliteConnection cn = new SqliteConnection(connectionString);
            SqliteCommand cmd = null;

            cn.Open();
            Console.WriteLine("Connection open: " + fi.FullName);

            cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS `Item` (`ObjectNo` INTEGER NOT NULL, `Name` TEXT NOT NULL, PRIMARY KEY(`ObjectNo`));", cn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Table 'Item' created");

            long? max;

            cmd = new SqliteCommand("SELECT MAX(ObjectNo) FROM Item", cn);
            max = cmd.ExecuteScalar() as long?;

            if (max == null)
            {
                max = 0;
            }
            else
            {
                max++;
            }

            cmd = new SqliteCommand("INSERT INTO `Item` (ObjectNo, Name) VALUES (@ObjectNo, @Name);", cn);
            cmd.Parameters.Add(new SqliteParameter() { ParameterName = "@ObjectNo", Value = max });
            cmd.Parameters.Add(new SqliteParameter() { ParameterName = "@Name", Value = "Item " + max });
            cmd.ExecuteNonQuery();
            Console.WriteLine("Item {0} inserted", max);

            cn.Close();
            Console.WriteLine("Connection closed");
        }
    }
}
