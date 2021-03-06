using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Core;
using Starcounter.Core.Database;
using StarDump.Common;

namespace StarDump.Core
{
    public class UnloadRow : StarDump.Common.DumpRow, Starcounter.Core.Abstractions.Database.IDbProxy
    {
        public UnloadRow(ulong dbId, ulong dbRef) : base(dbId, dbRef)
        {
        }

        public void Fill(UnloadTable table)
        {
            foreach (var c in table.Columns)
            {
                this[c.Name] = table.CrudHelper.GetValue(this.DbObjectIdentity, this.DbObjectReference, c.Name);
            }
        }

        /// <summary>
        /// Inserts current UnloadRow into database and sets it's value. Should be called inside a <see reference="Db.Transact">transaction</see>.
        /// </summary>
        public void Insert(ReloadTable table)
        {
            ulong dbHandle = Starcounter.Core.Database.Transaction.Current.DatabaseContext.Handle;
            ulong crudHandle;
            ulong dbId = this.DbGetIdentity();
            ulong dbRef;

            Db.MetalayerCheck(Starcounter.Core.Interop.sccrud.star_crud_GetCreateHandle(dbHandle, table.Name, out crudHandle));
            DbCrud.CreateWithId(dbId, out dbRef, crudHandle);

            foreach (ReloadColumn c in table.Columns)
            {
                table.CrudHelper.SetValue(dbId, dbRef, c.Name, this[c.Name]);
            }
        }

        #region Starcounter.Abstractions.Database.IDbProxy
        public ulong DbGetReference()
        {
            return this.DbObjectReference;
        }

        public ulong DbGetIdentity()
        {
            return this.DbObjectIdentity;
        }
        
        public void DbDelete()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Starcounter.Core.Abstractions.Database.IDbProxy obj)
        {
            return this.DbObjectIdentity == obj.DbGetIdentity();
        }
        #endregion
    }
}