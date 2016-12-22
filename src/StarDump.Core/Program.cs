using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Starcounter;
using StarDump.Common;

namespace StarDump.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Configuration config = new Configuration();
            //config.FileName = Path.Combine(@"C:\Temp\Core\Database", config.FileInfo.Name);
            //config.FileName = @"C:\Temp\Core\Database\stardump-default-2016.12.06-10.44.sqlite3";

            config.FileName = Path.Combine(@"D:\Temp\Core\Database", config.FileInfo.Name);
            // config.FileName = @"D:\Temp\Core\Database\stardump-default-2016.12.06-00.55.sqlite3";
            // config.DatabaseName = "reload";

            CommandInterface.Unload(config, new Output());
            //Reload(config);
        }
    }
}
