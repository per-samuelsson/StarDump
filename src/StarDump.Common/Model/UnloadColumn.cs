using System;

namespace StarDump.Common
{
    public abstract class UnloadColumn
    {
        public ulong ObjectId { get; protected set; }
        public string Name { get; protected set; }
        public string DataTypeName { get; protected set; }
        public bool Nullable { get; protected set; }
        public bool Inherited { get; protected set; }
    }
}