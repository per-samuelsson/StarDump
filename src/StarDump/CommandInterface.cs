﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump
{
    public class CommandInterface
    {
        internal static Output Out;

        public static bool Reload(Configuration config)
        {
            Out = config.Output;

            Reload reload = new Reload(config);

            reload.ReloadTableStart += (sender, table) =>
            {
                Out.WriteLine(string.Format("{0}: start {1}", DateTime.Now, table));
            };

            reload.ReloadTableFinish += (sender, table) =>
            {
                Out.WriteLine(string.Format("{0}: finish {1}", DateTime.Now, table));
            };

            RunResult result = reload.Run();

            Out.WriteLine("Reload finished " + config.FileName);
            Out.WriteLine("Elapsed " + result.Elapsed);
            Out.WriteLine("Tables count " + result.Tables);
            Out.WriteLine("Rows count " + result.Rows);

            return true;
        }

        public static bool Unload(Configuration config)
        {
            Out = config.Output;

            Unload unload = new Unload(config);

            unload.UnloadTableStart += (sender, table) =>
            {
                Out.WriteLine(string.Format("{0}: start {1}", DateTime.Now, table));
            };

            unload.UnloadTableFinish += (sender, table) =>
            {
                Out.WriteLine(string.Format("{0}: finish {1}", DateTime.Now, table));
            };

            RunResult result = unload.Run();

            Out.WriteLine("Unload finished " + config.FileName);
            Out.WriteLine("Elapsed " + result.Elapsed);
            Out.WriteLine("Tables count " + result.Tables);
            Out.WriteLine("Rows count " + result.Rows);

            return true;
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
