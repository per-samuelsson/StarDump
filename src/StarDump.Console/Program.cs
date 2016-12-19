using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StarDump;
using Microsoft.Extensions.CommandLineUtils;

namespace StarDump.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineInterface cli = new CommandLineInterface();
            cli.Execute(args);
        }
    }
}
