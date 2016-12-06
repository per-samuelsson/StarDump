using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StarDump;

namespace StarDump.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLine commandLine = new CommandLine();

            if (commandLine.Parse(args) == false)
            {
                Out.WriteErrorLine();
                Out.WriteErrorLine("Argument parsing failed.");
                return;
            }

            if (commandLine.ValidateArguments() == false)
            {
                Out.WriteErrorLine();
                Out.WriteErrorLine("Argument validation failed.");
                return;
            }

            if (commandLine.SetOptions() == false)
            {
                Out.WriteErrorLine();
                Out.WriteErrorLine("Setting command line options failed.");
                return;
            }

            if (commandLine.Run() == false)
            {
                Out.WriteErrorLine();
                Out.WriteErrorLine("Execution failed.");
                return;
            }
        }
    }
}
