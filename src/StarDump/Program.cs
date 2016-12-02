using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Starcounter;

namespace StarDump
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Configuration config = new Configuration();
            config.FileName = Path.Combine(@"D:\Temp\Core\Database", config.FileInfo.Name);

            Unload(config);
        }

        public static void Unload(Configuration config)
        {
            Unload unload = new Unload(config);

            unload.UnloadTableStart += (sender, table) =>
            {
                Console.WriteLine("{0}: start {1}", DateTime.Now, table.FullName);
            };

            unload.UnloadTableFinish += (sender, table) =>
            {
                Console.WriteLine("{0}: finish {1}", DateTime.Now, table.FullName);
            };

            RunResult result = unload.Run();

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
