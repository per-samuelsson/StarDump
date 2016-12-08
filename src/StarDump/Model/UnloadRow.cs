using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Database.Interop;

namespace StarDump
{
    public class UnloadRow : Dictionary<string, object>, Starcounter.Abstractions.Database.IDbProxy
    {
        public const string DbIdKey = "__dbId";
        public const string DbRefKey = "__dbRef";

        public static void RegisterDatabaseType()
        {
            Db.BindDatabaseClass(typeof(UnloadRow), (dbId, dbRef) => 
            {
                return new UnloadRow(dbId, dbRef);
            });
        }

        public UnloadRow(ulong dbId, ulong dbRef)
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

        /// <summary>
        /// Inserts current ResultRow into database and sets it's value. Should be called inside a <see reference="Db.Transact">transaction</see>.
        /// </summary>
        public void Insert(string tableName, ReloadColumn[] columns)
        {
            ulong dbHandle = Starcounter.Database.Transaction.Current.DatabaseContext.Handle;
            ulong crudHandle;
            ulong dbId = this.DbGetIdentity();
            ulong dbRef;

            Db.MetalayerCheck(Starcounter.Database.Interop.sccrud.star_crud_GetCreateHandle(dbHandle, tableName, out crudHandle));
            DbCrud.CreateWithId(dbId, out dbRef, crudHandle);

            foreach (ReloadColumn c in columns)
            {
                ulong setter;
                object value = this[c.Name];
            
                Db.MetalayerCheck(Starcounter.Database.Interop.sccrud.star_crud_GetPropertyWriteHandle(dbHandle, tableName, c.Name, out setter));
                this.SetValue(dbId, dbRef, setter, c.DataType, c.Nullable, value);
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
                case "ulong": return DbCrud.GetULong(dbId, dbRef, getter);
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

                    return m.GetObjectNo();
                case "ulong?": return DbCrud.GetNullableULong(dbId, dbRef, getter);
                case "ushort?": return DbCrud.GetNullableUShort(dbId, dbRef, getter);
                default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
            }
        }

        protected void SetValue(ulong dbId, ulong dbRef, ulong setter, string dataTypeName, bool nullable, object value)
        {
            if (nullable)
            {
                dataTypeName += "?";
            }

            switch (dataTypeName)
            {
                case "bool": DbCrud.SetBool(dbId, dbRef, setter, (bool)value); break;
                case "byte": DbCrud.SetByte(dbId, dbRef, setter, (byte)value); break;
                case "char": DbCrud.SetChar(dbId, dbRef, setter, (char)value); break;
                case "DateTime": DbCrud.SetDateTime(dbId, dbRef, setter, (DateTime)value); break;
                case "decimal": DbCrud.SetDecimal(dbId, dbRef, setter, (decimal)value); break;
                case "double": DbCrud.SetDouble(dbId, dbRef, setter, (double)value); break;
                case "float": DbCrud.SetFloat(dbId, dbRef, setter, (float)value); break;
                case "int": DbCrud.SetInt(dbId, dbRef, setter, (int)value); break;
                case "long": DbCrud.SetLong(dbId, dbRef, setter, (long)value); break;
                case "sbyte": DbCrud.SetSByte(dbId, dbRef, setter, (sbyte)value); break;
                case "short": DbCrud.SetShort(dbId, dbRef, setter, (short)value); break;
                case "string?":
                case "string": DbCrud.SetString(dbId, dbRef, setter, (string)value); break;
                case "uint": DbCrud.SetUInt(dbId, dbRef, setter, (uint)value); break;
                case "ulong": DbCrud.SetULong(dbId, dbRef, setter, (ulong)value); break;
                case "ushort": DbCrud.SetUShort(dbId, dbRef, setter, (ushort)value); break;
                case "byte[]": DbCrud.SetBinary(dbId, dbRef, setter, (byte[])value); break;

                case "bool?": DbCrud.SetNullableBool(dbId, dbRef, setter, value as bool?); break;
                case "byte?": DbCrud.SetNullableByte(dbId, dbRef, setter, value as byte?); break;
                case "char?": DbCrud.SetNullableChar(dbId, dbRef, setter, value as char?); break;
                case "DateTime?": DbCrud.SetNullableDateTime(dbId, dbRef, setter, value as DateTime?); break;
                case "decimal?": DbCrud.SetNullableDecimal(dbId, dbRef, setter, value as decimal?); break;
                case "double?": DbCrud.SetNullableDouble(dbId, dbRef, setter, value as double?); break;
                case "float?": DbCrud.SetNullableFloat(dbId, dbRef, setter, value as float?); break;
                case "int?": DbCrud.SetNullableInt(dbId, dbRef, setter, value as int?); break;
                case "long?": DbCrud.SetNullableLong(dbId, dbRef, setter, value as long?); break;
                case "sbyte?": DbCrud.SetNullableSByte(dbId, dbRef, setter, value as sbyte?); break;
                case "short?": DbCrud.SetNullableShort(dbId, dbRef, setter, value as short?); break;
                case "uint?": DbCrud.SetNullableUInt(dbId, dbRef, setter, value as uint?); break;
                case "reference":
                case "reference?":
                    ulong? parentDbId = value as ulong?;
                    DbCrud.SetDb(dbId, dbRef, setter, parentDbId);
                    break;
                case "ulong?": DbCrud.SetNullableULong(dbId, dbRef, setter, value as ulong?); break;
                case "ushort?": DbCrud.SetNullableUShort(dbId, dbRef, setter, value as ushort?); break;
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