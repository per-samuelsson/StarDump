using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Database.Interop;

namespace StarDump
{
    public class CrudHelper
    {
        public string TableName { get; protected set; }
        protected Dictionary<string, ulong> getters = new Dictionary<string, ulong>();

        public CrudHelper(ulong dbHandle, UnloadTable table)
        {
            this.TableName = table.FullName;

            foreach (UnloadColumn c in table.Columns)
            {
                ulong getter;

                sccrud.star_crud_GetPropertyReadHandle(dbHandle, this.TableName, c.Name, out getter);
                getters.Add(c.Name, getter);
            }
        }

        public ulong GetGetter(string columnName)
        {
            return getters[columnName];
        }
    }
}