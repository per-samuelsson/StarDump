using System;
using System.Collections.Generic;
using System.Linq;

namespace StarDump.Common
{
    public abstract class DumpRow : Dictionary<string, object>
    {
        public ulong DbObjectIdentity { get; protected set; }
        public ulong DbObjectReference { get; protected set; }

        public DumpRow(ulong dbId, ulong dbRef)
        {
            this.DbObjectIdentity = dbId;
            this.DbObjectReference = dbRef;
        }
    }
}