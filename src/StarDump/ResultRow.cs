using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Database.Interop;

namespace StarDump
{
    public class ResultRow : Dictionary<string, object>, Starcounter.Abstractions.Database.IDbProxy
    {
        public const string DbIdKey = "__dbId";
        public const string DbRefKey = "__dbRef";

        public static void RegisterDatabaseType()
        {
            Db.BindDatabaseClass(typeof(ResultRow), (dbId, dbRef) => 
            {
                return new ResultRow(dbId, dbRef);
            });
        }

        public ResultRow(ulong dbId, ulong dbRef)
        {
            this[DbIdKey] = dbId;
            this[DbRefKey] = dbRef;
        }

        public void Fill(string tableName, Starcounter.Metadata.Column[] columns)
        {
            ulong dbId = this.DbGetIdentity();
            ulong dbRef = this.DbGetReference();
            ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;

            foreach (var c in columns)
            {
                ulong getter;
                object value;
                
                sccrud.star_crud_GetPropertyReadHandle(dbHandle, tableName, c.Name, out getter);
                value = this.GetValue(dbId, dbRef, getter, c.DataType.Name, c.Nullable);
                
                this[c.Name] = value;
            }
        }

        protected object GetValue(ulong dbId, ulong dbRef, ulong getter, string dataTypeName, bool nullable)
        {
            if (nullable)
            {
                dataTypeName += "?";
            }

            switch (dataTypeName)
            {
                case "bool": return DbCrud.GetBool(dbId, dbRef, getter);
                case "byte": return DbCrud.GetByte(dbId, dbRef, getter);
                case "char": return DbCrud.GetChar(dbId, dbRef, getter);
                case "DateTime": return DbCrud.GetDateTime(dbId, dbRef, getter);
                case "decimal": return DbCrud.GetDecimal(dbId, dbRef, getter);
                case "double": return DbCrud.GetDouble(dbId, dbRef, getter);
                case "float": return DbCrud.GetFloat(dbId, dbRef, getter);
                case "int": return DbCrud.GetInt(dbId, dbRef, getter);
                case "long": return DbCrud.GetLong(dbId, dbRef, getter);
                case "sbyte": return DbCrud.GetSByte(dbId, dbRef, getter);
                case "short": return DbCrud.GetShort(dbId, dbRef, getter);
                case "string?":
                case "string": return DbCrud.GetString(dbId, dbRef, getter);
                case "uint": return DbCrud.GetUInt(dbId, dbRef, getter);
                case "ulong": return (long)DbCrud.GetULong(dbId, dbRef, getter);
                case "ushort": return DbCrud.GetUShort(dbId, dbRef, getter);
                case "byte[]": return DbCrud.GetBinary(dbId, dbRef, getter);

                case "bool?": return DbCrud.GetNullableBool(dbId, dbRef, getter);
                case "byte?": return DbCrud.GetNullableByte(dbId, dbRef, getter);
                case "char?": return DbCrud.GetNullableChar(dbId, dbRef, getter);
                case "DateTime?": return DbCrud.GetNullableDateTime(dbId, dbRef, getter);
                case "decimal?": return DbCrud.GetNullableDecimal(dbId, dbRef, getter);
                case "double?": return DbCrud.GetNullableDouble(dbId, dbRef, getter);
                case "float?": return DbCrud.GetNullableFloat(dbId, dbRef, getter);
                case "int?": return DbCrud.GetNullableInt(dbId, dbRef, getter);
                case "long?": return DbCrud.GetNullableLong(dbId, dbRef, getter);
                case "sbyte?": return DbCrud.GetNullableSByte(dbId, dbRef, getter);
                case "short?": return DbCrud.GetNullableShort(dbId, dbRef, getter);
                case "uint?": return DbCrud.GetNullableUInt(dbId, dbRef, getter);
                case "reference":
                case "reference?":
                    var m = DbCrud.GetDb<Starcounter.Internal.Metadata.MotherOfAllLayouts>(dbId, dbRef, getter);

                    if (m == null)
                    {
                        return null;
                    }

                    return (long)m.GetObjectNo();
                case "ulong?": return (long?)DbCrud.GetNullableULong(dbId, dbRef, getter);
                case "ushort?": return DbCrud.GetNullableUShort(dbId, dbRef, getter);
                default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
            }
        }

        #region Starcounter.Abstractions.Database.IDbProxy
        public ulong DbGetReference()
        {
            return (ulong)this[DbRefKey];
        }

        public ulong DbGetIdentity()
        {
            return (ulong)this[DbIdKey];
        }
        
        public void DbDelete()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Starcounter.Abstractions.Database.IDbProxy obj)
        {
            return this.DbGetIdentity() == obj.DbGetIdentity();
        }
        #endregion
    }
}