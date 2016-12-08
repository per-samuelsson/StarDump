using System;

namespace StarDump
{
    public class RunResult
    {
        public TimeSpan Elapsed { get; protected set; }
        public int Tables { get; protected set; }
        public ulong Rows { get; protected set; }

        public RunResult(TimeSpan elapsed, int tables, ulong rows)
        {
            this.Elapsed = elapsed;
            this.Tables = tables;
            this.Rows = rows;
        }
    }
}