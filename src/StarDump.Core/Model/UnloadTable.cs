using System;
using System.Collections.Generic;
using Starcounter;
using StarDump.Common;

namespace StarDump.Core
{
    public class UnloadTable : StarDump.Common.UnloadTable
    {
        /// <summary>
        /// Temp variable to save CrudHelp after all the columns add to the <see cref="UnloadTable.Columns">Columns</see> list
        /// </summary>
        public CrudHelper CrudHelper { get; set; }
        public Starcounter.Metadata.RawView RawView { get; protected set; }

        public UnloadTable(string setSpecifier, Starcounter.Metadata.RawView view)
        {
            this.RawView = view;
            this.ObjectId = view.GetObjectNo();
            this.FullName = view.FullName;
            this.SetSpecifier = setSpecifier;

            this.Columns = new List<StarDump.Common.UnloadColumn>();
            this.Rows = new List<DumpRow>();
            this.RowsCount = 0;
        }

        public override ulong? GetParentId()
        {
            if (this.RawView?.Inherits != null)
            {
                return this.RawView.Inherits.GetObjectNo();
            }

            return null;
        }

        public void FillRow(DumpRow row)
        {
            foreach (var c in this.Columns)
            {
                row[c.Name] = this.CrudHelper.GetValue(row.DbObjectIdentity, row.DbObjectReference, c.Name);
            }
        }
    }
}