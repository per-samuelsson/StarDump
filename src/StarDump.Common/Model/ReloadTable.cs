using System;
using System.Collections.Generic;

namespace StarDump.Common
{
    public abstract class ReloadTable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public string ParentName { get; set; }

        /// <summary>
        /// Temp counter to save total number of rows
        /// </summary>
        public ulong RowsCount { get; set; }
        public List<ReloadColumn> Columns { get; set; }
        public List<ReloadTable> Children { get; set; }

        public ReloadTable()
        {
            this.Children = new List<ReloadTable>();
        }
    }
}