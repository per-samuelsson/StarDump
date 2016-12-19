using System;
using Starcounter;

namespace StarDump.Core
{
    public class UnloadColumn
    {
        public ulong ObjectId { get; protected set; }
        public string Name { get; protected set; }
        public string DataTypeName { get; protected set; }
        public bool Nullable { get; protected set; }
        public bool Inherited { get; protected set; }

        public UnloadColumn(Starcounter.Metadata.Column column)
        {
            this.ObjectId = column.GetObjectNo();
            this.Name = column.Name;
            this.DataTypeName = column.DataType.Name;
            this.Nullable = column.Nullable;
            this.Inherited = column.Inherited;
        }
    }
}