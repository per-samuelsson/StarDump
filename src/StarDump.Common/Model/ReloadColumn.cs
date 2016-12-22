using System;

namespace StarDump.Common
{
    public class ReloadColumn
    {
        public long Id { get; set; }
        public long TableId { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string ReferenceType { get; set; }
        public bool Nullable { get; set; }
        public bool Inherited { get; set; }
    }
}