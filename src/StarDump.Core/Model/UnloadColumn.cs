namespace StarDump.Core
{
    using Starcounter.Core;

    public class UnloadColumn : StarDump.Common.UnloadColumn
    {
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