using System;
using System.Collections.Generic;

namespace StarDump.Common
{
    public abstract class UnloadTable
    {
        public ulong ObjectId { get; protected set; }
        public string FullName { get; protected set; }
        public string SetSpecifier { get; protected set; }
        public List<UnloadColumn> Columns { get; protected set; }

        /// <summary>
        /// Temp lilst to save rows of the table to insert into Sqlite
        /// </summary>
        public List<DumpRow> Rows { get; set; }

        /// <summary>
        /// Temp counter to save total number of rows
        /// </summary>
        public ulong RowsCount { get; set; }

        /// <summary>
        /// Temp variable to save INSERT INTO SQL statment
        /// </summary>
        public string InsertIntoDefinition { get; set; }

        public abstract ulong? GetParentId();
    }
}