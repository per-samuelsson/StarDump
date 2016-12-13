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
        public List<UnloadRow> Rows { get; protected set; }
        public ulong RowsCount { get; set; }
        public string InsertIntoDefinition { get; set; }
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