namespace StarDump.Core
{
    public enum MetaInfoType
    {
        UnloadStart = 1,
        UnloadFinish = 2,
        TablesCount = 3,
        RowsCount = 4,
        StarcounterVersion = 5,
        StarDumpVersion = 6,
        FormatVersion = 7,
        DatabaseName = 8,
        SkipTablePrefixes = 9,
        SkipColumnPrefixes = 10
    }
}