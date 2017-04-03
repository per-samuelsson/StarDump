using System;
using System.IO;

namespace StarDump.Common
{
    /// <summary>
    /// Configuration interface between <see cref="StarDump"/> CLI and <see cref="StarDump.Core"/>
    /// </summary>
    public class Configuration
    {
        public int Verbose { get; set; }
        public string DatabaseName { get; set; }
        public string FileName { get; set; }
        public string[] SkipColumnPrefixes { get; set; }
        public string[] SkipTablePrefixes { get; set; }
        public int InsertRowsBufferSize { get; set; }
        /// <summary>
        /// Forces the file to be reloaded even if the database already contains data.
        /// If true, the user has to make sure that all object ID:s are unique manually.
        /// 
        /// Default: false
        /// </summary>
        public bool ForceReload { get; set; }

        public Configuration() : this("default")
        {
        }

        public Configuration(string databaseName)
        {
            this.Verbose = 1;
            this.DatabaseName = databaseName;
            this.SkipColumnPrefixes = new string[] { "__" };
            this.SkipTablePrefixes = new string[] { };
            this.InsertRowsBufferSize = 500;
            this.ForceReload = false;


            string name = string.Format("stardump-{0}-{1}.sqlite3", this.DatabaseName, DateTime.Now.ToString("yyyy.MM.dd-HH.mm"));
            string path = Path.GetTempPath();

            this.FileName = Path.Combine(path, name);
        }

        public FileInfo FileInfo
        {
            get
            {
                return new FileInfo(this.FileName);
            }
        }
    }
}