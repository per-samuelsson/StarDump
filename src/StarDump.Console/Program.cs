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
            // command for running as before:
            //    dotnet run unload
            // or set Application Arguments @ StarDump->Properties->Debug to unload

            CommandLine commandLine = new CommandLine();

            if (commandLine.Parse(args) == false)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Argument parsing failed.");
                return;
            }

            if (commandLine.ValidateArguments() == false)
            {
            }

            if (commandLine.Run() == false)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Execution failed.");
                return;
            }
        }
    }
}
