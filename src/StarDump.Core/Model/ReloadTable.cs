using System;
using System.Collections.Generic;
using StarDump.Common;

namespace StarDump.Core
{
    public class ReloadTable : StarDump.Common.ReloadTable
    {
        public CrudHelper CrudHelper { get; set; }

        public ReloadTable()
        {
            this.Children = new List<StarDump.Common.ReloadTable>();
        }
    }
}