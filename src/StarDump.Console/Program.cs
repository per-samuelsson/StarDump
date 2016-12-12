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
                commandLine.Out.WriteErrorLine();
                commandLine.Out.WriteErrorLine("Argument parsing failed.");
                return;
            }

            if (commandLine.ValidateArguments() == false)
            {
                commandLine.Out.WriteErrorLine();
                commandLine.Out.WriteErrorLine("Argument validation failed.");
                return;
            }

            if (commandLine.SetOptions() == false)
            {
                commandLine.Out.WriteErrorLine();
                commandLine.Out.WriteErrorLine("Setting command line options failed.");
                return;
            }

            if (commandLine.Run() == false)
            {
                commandLine.Out.WriteErrorLine();
                commandLine.Out.WriteErrorLine("Execution failed.");
                return;
            }
        }
    }
}
