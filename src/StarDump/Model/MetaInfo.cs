namespace StarDump
{
    public class MetaInfo
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Value { get; set; }

        public MetaInfo(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public MetaInfo(MetaInfoType type, string name) : this((int)type, name)
        {
        }

        public MetaInfo(int id, string name, string value) : this(id, name)
        {
            this.Value = value;
        }

        public MetaInfo(MetaInfoType type, string name, string value) : this((int)type, name, value)
        {
        }

        public static MetaInfo UnloadStart
        {
            get
            {
                return new MetaInfo(MetaInfoType.UnloadStart, "Unload start");
            }
        }

        public static MetaInfo UnloadFinish
        {
            get
            {
                return new MetaInfo(MetaInfoType.UnloadFinish, "Unload finish");
            }
        }

        public static MetaInfo TablesCount
        {
            get
            {
                return new MetaInfo(MetaInfoType.TablesCount, "Total number of data tables without meta tables");
            }
        }

        public static MetaInfo RowsCount
        {
            get
            {
                return new MetaInfo(MetaInfoType.RowsCount, "Total number of data rows without meta tables");
            }
        }

        public static MetaInfo StarcounterVersion
        {
            get
            {
                return new MetaInfo(MetaInfoType.StarcounterVersion, "Starcounter version");
            }
        }

        public static MetaInfo StarDumpVersion
        {
            get
            {
                return new MetaInfo(MetaInfoType.StarDumpVersion, "StarDump version");
            }
        }

        public static MetaInfo FormatVersion
        {
            get
            {
                return new MetaInfo(MetaInfoType.FormatVersion, "Unload format version");
            }
        }

        public static MetaInfo DatabaseName
        {
            get
            {
                return new MetaInfo(MetaInfoType.DatabaseName, "Database name");
            }
        }

        public static MetaInfo SkipTablePrefixes
        {
            get
            {
                return new MetaInfo(MetaInfoType.SkipTablePrefixes, "Skip table prefixes");
            }
        }

        public static MetaInfo SkipColumnPrefixes
        {
            get
            {
                return new MetaInfo(MetaInfoType.SkipColumnPrefixes, "Skip column prefixes");
            }
        }
    }
}