namespace StarDump.Core
{
    public static class Assembly
    {
        /// <summary>
        /// Version of Application
        /// </summary>
        public const string Version = "0.1.0";
		
        /// <summary>
        /// Version of SQLite dump format
        /// </summary>
        public const string FormatVersion = "0.0.1";

        public static string ApplicationVersionMessage()
        {
            return string.Format("{0, -30}{1, 8}", "StarDump application version: ", Version);
        }

        public static string FormatVersionMessage()
        {
            return string.Format("{0, -30}{1, 8}", "SQLite dump format version: ", FormatVersion);
        }
    }
}