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

            if (result == null)
            {
                return false;
            }

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
    }
}
