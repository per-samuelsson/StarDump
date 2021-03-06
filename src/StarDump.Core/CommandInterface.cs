﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StarDump.Common;

namespace StarDump.Core
{
    public class CommandInterface
    {
        internal static Output Out;

        public static bool Reload(Configuration config, Output output)
        {
            Out = output;

            Reload reload = new Reload(config);

            if (config.Verbose > 0)
            {
                reload.ReloadTableStart += (sender, table) =>
                {
                    Out.WriteLine(string.Format("{0}: start {1}", DateTime.Now, table));
                };

                reload.ReloadTableFinish += (sender, table) =>
                {
                    Out.WriteLine(string.Format("{0}: finish {1}", DateTime.Now, table));
                };
            }

            reload.ErrorEvent += (sender, message) =>
            {
                Out.WriteErrorLine(message);
            };
            reload.WarningEvent += (sender, message) =>
            {
                Out.WriteWarningLine(message);
            };

            Out.WriteLine("Reload started " + config.FileName);

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

        public static bool Unload(Configuration config, Output output)
        {
            Out = output;

            Unload unload = new Unload(config);

            if (config.Verbose > 0)
            {
                unload.RowsChunkUnloaded += (sender, table) =>
                {
                    Out.WriteLine(string.Format("{0}: {1} {2} rows unloaded", DateTime.Now, config.InsertRowsBufferSize, table));
                };
            }

            Out.WriteLine("Unload started " + config.FileName);

            RunResult result = unload.Run();

            Out.WriteLine("Unload finished " + config.FileName);
            Out.WriteLine("Elapsed " + result.Elapsed);
            Out.WriteLine("Tables count " + result.Tables);
            Out.WriteLine("Rows count " + result.Rows);

            return true;
        }
    }
}