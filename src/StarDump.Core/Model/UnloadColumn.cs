using System;
using System.Collections.Generic;
using System.Linq;

namespace StarDump.Core
{
    using Starcounter.Core;

    public class UnloadColumn : StarDump.Common.UnloadColumn
    {
        public string OriginalDataTypeName { get; protected set; }
        public string FullDataTypeName { get; protected set; }

        public UnloadColumn(Starcounter.Metadata.Column column, StarDump.Common.SqlHelper sqlHelper)
        {
            this.ObjectId = column.GetObjectNo();
            this.Name = column.Name;
            this.DataTypeName = column.DataType.Name;
            this.OriginalDataTypeName = this.DataTypeName;
            this.Nullable = column.Nullable;
            this.Inherited = column.Inherited;

            if (!sqlHelper.IsSupportedDataType(this.DataTypeName))
            {
                Starcounter.Metadata.RawView view = Db.SQL<Starcounter.Metadata.RawView>("SELECT r FROM Starcounter.Metadata.RawView r").FirstOrDefault(x => x.Name == this.DataTypeName);

                if (view != null)
                {
                    this.DataTypeName = "reference";
                    this.FullDataTypeName = view.FullName;
                }
            }
        }
    }
}