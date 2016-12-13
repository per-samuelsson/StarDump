using System;
using System.Collections.Generic;
using Starcounter;

namespace StarDump
{
    public class UnloadTable
    {
        public ulong ObjectId { get; protected set; }
        public string FullName { get; protected set; }
        public string SetSpecifier { get; protected set; }
        public Starcounter.Metadata.RawView RawView { get; protected set; }
        public List<UnloadColumn> Columns { get; protected set; }

        /// <summary>
        /// Temp lilst to save rows of the table to insert into Sqlite
        /// </summary>
        public List<UnloadRow> Rows { get; set; }

        /// <summary>
        /// Temp counter to save total number of rows
        /// </summary>
        public ulong RowsCount { get; set; }

        /// <summary>
        /// Temp variable to save INSERT INTO SQL statment
        /// </summary>
        public string InsertIntoDefinition { get; set; }

        /// <summary>
        /// Temp variable to save CrudHelp after all the columns add to the <see cref="UnloadTable.Columns">Columns</see> list
        /// </summary>
        public CrudHelper CrudHelper { get; set; }

        public UnloadTable(string setSpecifier, Starcounter.Metadata.RawView view)
        {
            this.RawView = view;
            this.ObjectId = view.GetObjectNo();
            this.FullName = view.FullName;
            this.SetSpecifier = setSpecifier;

            this.Columns = new List<UnloadColumn>();
            this.Rows = new List<UnloadRow>();
            this.RowsCount = 0;
        }
    }
}