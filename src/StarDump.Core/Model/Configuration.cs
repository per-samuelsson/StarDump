using System;
using System.IO;

namespace StarDump.Core
{
    public class Configuration
    {
        public Configuration()
        {
            this.Verbose = 1;
            this.DatabaseName = "default";
            this.SkipColumnPrefixes = new string[] { "__" };
            this.SkipTablePrefixes = new string[] { "Starcounter.", "Concepts.", "SocietyObjects." };
            this.InsertRowsBufferSize = 500;

            string name = string.Format("stardump-{0}-{1}.sqlite3", this.DatabaseName, DateTime.Now.ToString("yyyy.MM.dd-HH.mm"));
            string path = Path.GetTempPath();

            this.FileName = Path.Combine(path, name);

            this.Output = new Output();
        }

        public int Verbose { get; set; }
        public string DatabaseName { get; set; }
        public string FileName { get; set; }
        public string[] SkipColumnPrefixes { get; set; }
        public string[] SkipTablePrefixes { get; set; }
        public int InsertRowsBufferSize { get; set; }
        public Output Output { get; set; }

        public FileInfo FileInfo
        {
            get
            {
                return new FileInfo(this.FileName);
            }
        }
    }
}