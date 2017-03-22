using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Core;
using Starcounter.Core.Interop;
using Starcounter.Core.Database;
using StarDump.Common;

namespace StarDump.Core
{
    public class CrudHelper
    {
        public string TableName { get; protected set; }
        protected Dictionary<string, ulong> getters = new Dictionary<string, ulong>();
        protected Dictionary<string, ulong> setters = new Dictionary<string, ulong>();
        protected Dictionary<string, Func<ulong, ulong, ulong, object>> getMethods = new Dictionary<string, Func<ulong, ulong, ulong, object>>();
        protected Dictionary<string, Action<ulong, ulong, ulong, object>> setMethods = new Dictionary<string, Action<ulong, ulong, ulong, object>>();

        public CrudHelper(ulong dbHandle, UnloadTable table)
        {
            this.TableName = table.FullName;

            foreach (UnloadColumn c in table.Columns)
            {
                ulong getter;

                Db.MetalayerCheck(sccrud.star_crud_GetPropertyReadHandle(dbHandle, this.TableName, c.Name, out getter));
                System.Diagnostics.Debug.Assert(getter > 0);

                getters.Add(c.Name, getter);
                getMethods.Add(c.Name, this.GetMethod(c.DataTypeName, c.Nullable));
            }
        }

        public CrudHelper(ulong dbHandle, ReloadTable table)
        {
            this.TableName = table.Name;

            foreach (ReloadColumn c in table.Columns)
            {
                ulong setter;

                Db.MetalayerCheck(Starcounter.Core.Interop.sccrud.star_crud_GetPropertyWriteHandle(dbHandle, this.TableName, c.Name, out setter));
                System.Diagnostics.Debug.Assert(setter > 0);

                setters.Add(c.Name, setter);
                setMethods.Add(c.Name, this.SetMethod(c.DataTypeName, c.Nullable));
            }
        }

        public ulong GetGetter(string columnName)
        {
            return getters[columnName];
        }

        public ulong GetSetter(string columnName)
        {
            return setters[columnName];
        }

        public object GetValue(ulong dbId, ulong dbRef, string columnName)
        {
            ulong getter = getters[columnName];
            Func<ulong, ulong, ulong, object> method = this.getMethods[columnName];
            object value = method(dbId, dbRef, getter);

            return value;
        }

        public void SetValue(ulong dbId, ulong dbRef, string columnName, object value)
        {
            ulong setter = setters[columnName];
            Action<ulong, ulong, ulong, object> method = this.setMethods[columnName];
            method(dbId, dbRef, setter, value);
        }

        protected Action<ulong, ulong, ulong, object> SetMethod(string dataTypeName, bool nullable)
        {
            if (nullable)
            {
                switch (dataTypeName)
                {
                    case "string": return (dbId, dbRef, setter, value) => DbCrud.SetString(dbId, dbRef, setter, value as string);
                    case "bool": return (dbId, dbRef, setter, value) => DbCrud.SetNullableBool(dbId, dbRef, setter, value as bool?);
                    case "byte": return (dbId, dbRef, setter, value) => DbCrud.SetNullableByte(dbId, dbRef, setter, value as byte?);
                    case "char": return (dbId, dbRef, setter, value) => DbCrud.SetNullableChar(dbId, dbRef, setter, value as char?);
                    case "DateTime": return (dbId, dbRef, setter, value) => DbCrud.SetNullableDateTime(dbId, dbRef, setter, value as DateTime?);
                    case "decimal": return (dbId, dbRef, setter, value) => DbCrud.SetNullableDecimal(dbId, dbRef, setter, value as decimal?);
                    case "double": return (dbId, dbRef, setter, value) => DbCrud.SetNullableDouble(dbId, dbRef, setter, value as double?);
                    case "float": return (dbId, dbRef, setter, value) => DbCrud.SetNullableFloat(dbId, dbRef, setter, value as float?);
                    case "int": return (dbId, dbRef, setter, value) => DbCrud.SetNullableInt(dbId, dbRef, setter, value as int?);
                    case "long": return (dbId, dbRef, setter, value) => DbCrud.SetNullableLong(dbId, dbRef, setter, value as long?);
                    case "sbyte": return (dbId, dbRef, setter, value) => DbCrud.SetNullableSByte(dbId, dbRef, setter, value as sbyte?);
                    case "short": return (dbId, dbRef, setter, value) => DbCrud.SetNullableShort(dbId, dbRef, setter, value as short?);
                    case "uint": return (dbId, dbRef, setter, value) => DbCrud.SetNullableUInt(dbId, dbRef, setter, value as uint?);
                    case "reference": return (dbId, dbRef, setter, value) => 
                    {
                        ulong? parentDbId = value as ulong?;
                        DbCrud.SetDb(dbId, dbRef, setter, parentDbId);
                    };
                    case "ulong": return (dbId, dbRef, setter, value) => DbCrud.SetNullableULong(dbId, dbRef, setter, value as ulong?);
                    case "ushort": return (dbId, dbRef, setter, value) => DbCrud.SetNullableUShort(dbId, dbRef, setter, value as ushort?);
                    default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
                }
            }

            switch (dataTypeName)
            {
                case "bool": return (dbId, dbRef, setter, value) => DbCrud.SetBool(dbId, dbRef, setter, (bool)value);
                case "byte": return (dbId, dbRef, setter, value) => DbCrud.SetByte(dbId, dbRef, setter, (byte)value);
                case "char": return (dbId, dbRef, setter, value) => DbCrud.SetChar(dbId, dbRef, setter, (char)value);
                case "DateTime": return (dbId, dbRef, setter, value) => DbCrud.SetDateTime(dbId, dbRef, setter, (DateTime)value);
                case "decimal": return (dbId, dbRef, setter, value) => DbCrud.SetDecimal(dbId, dbRef, setter, (decimal)value);
                case "double": return (dbId, dbRef, setter, value) => DbCrud.SetDouble(dbId, dbRef, setter, (double)value);
                case "float": return (dbId, dbRef, setter, value) => DbCrud.SetFloat(dbId, dbRef, setter, (float)value);
                case "int": return (dbId, dbRef, setter, value) => DbCrud.SetInt(dbId, dbRef, setter, (int)value);
                case "long": return (dbId, dbRef, setter, value) => DbCrud.SetLong(dbId, dbRef, setter, (long)value);
                case "sbyte": return (dbId, dbRef, setter, value) => DbCrud.SetSByte(dbId, dbRef, setter, (sbyte)value);
                case "short": return (dbId, dbRef, setter, value) => DbCrud.SetShort(dbId, dbRef, setter, (short)value);
                case "string": return (dbId, dbRef, setter, value) => DbCrud.SetString(dbId, dbRef, setter, (string)value);
                case "uint": return (dbId, dbRef, setter, value) => DbCrud.SetUInt(dbId, dbRef, setter, (uint)value);
                case "ulong": return (dbId, dbRef, setter, value) => DbCrud.SetULong(dbId, dbRef, setter, (ulong)value);
                case "ushort": return (dbId, dbRef, setter, value) => DbCrud.SetUShort(dbId, dbRef, setter, (ushort)value);
                case "byte[]": return (dbId, dbRef, setter, value) => DbCrud.SetBinary(dbId, dbRef, setter, (byte[])value);
                case "reference": return (dbId, dbRef, setter, value) =>
                {
                    ulong? parentDbId = value as ulong?;
                    DbCrud.SetDb(dbId, dbRef, setter, parentDbId);
                };
                default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
            }
        }

        protected Func<ulong, ulong, ulong, object> GetMethod(string dataTypeName, bool nullable)
        {
            if (nullable)
            {
                switch (dataTypeName)
                {
                    case "string": return (dbId, dbRef, getter) => DbCrud.GetString(dbId, dbRef, getter);
                    case "byte[]": return (dbId, dbRef, getter) => DbCrud.GetBinary(dbId, dbRef, getter);
                    case "bool": return (dbId, dbRef, getter) => DbCrud.GetNullableBool(dbId, dbRef, getter);
                    case "byte": return (dbId, dbRef, getter) => DbCrud.GetNullableByte(dbId, dbRef, getter);
                    case "char": return (dbId, dbRef, getter) => DbCrud.GetNullableChar(dbId, dbRef, getter);
                    case "DateTime": return (dbId, dbRef, getter) => DbCrud.GetNullableDateTime(dbId, dbRef, getter);
                    case "decimal": return (dbId, dbRef, getter) => DbCrud.GetNullableDecimal(dbId, dbRef, getter);
                    case "double": return (dbId, dbRef, getter) => DbCrud.GetNullableDouble(dbId, dbRef, getter);
                    case "float": return (dbId, dbRef, getter) => DbCrud.GetNullableFloat(dbId, dbRef, getter);
                    case "int": return (dbId, dbRef, getter) => DbCrud.GetNullableInt(dbId, dbRef, getter);
                    case "long": return (dbId, dbRef, getter) => DbCrud.GetNullableLong(dbId, dbRef, getter);
                    case "sbyte": return (dbId, dbRef, getter) => DbCrud.GetNullableSByte(dbId, dbRef, getter);
                    case "short": return (dbId, dbRef, getter) => DbCrud.GetNullableShort(dbId, dbRef, getter);
                    case "uint": return (dbId, dbRef, getter) => DbCrud.GetNullableUInt(dbId, dbRef, getter);
                    case "reference":
                        return (dbId, dbRef, getter) => 
                        {
                            var m = DbCrud.GetDb<Starcounter.Internal.Metadata.MotherOfAllLayouts>(dbId, dbRef, getter);

                            if (m == null)
                            {
                                return null;
                            }

                            return m.GetObjectNo();
                        };
                    case "ulong": return (dbId, dbRef, getter) => DbCrud.GetNullableULong(dbId, dbRef, getter);
                    case "ushort": return (dbId, dbRef, getter) => DbCrud.GetNullableUShort(dbId, dbRef, getter);
                    default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
                }                
            }

            switch (dataTypeName)
            {
                case "bool": return (dbId, dbRef, getter) => DbCrud.GetBool(dbId, dbRef, getter);
                case "byte": return (dbId, dbRef, getter) => DbCrud.GetByte(dbId, dbRef, getter);
                case "char": return (dbId, dbRef, getter) => DbCrud.GetChar(dbId, dbRef, getter);
                case "DateTime": return (dbId, dbRef, getter) => DbCrud.GetDateTime(dbId, dbRef, getter);
                case "decimal": return (dbId, dbRef, getter) => DbCrud.GetDecimal(dbId, dbRef, getter);
                case "double": return (dbId, dbRef, getter) => DbCrud.GetDouble(dbId, dbRef, getter);
                case "float": return (dbId, dbRef, getter) => DbCrud.GetFloat(dbId, dbRef, getter);
                case "int": return (dbId, dbRef, getter) => DbCrud.GetInt(dbId, dbRef, getter);
                case "long": return (dbId, dbRef, getter) => DbCrud.GetLong(dbId, dbRef, getter);
                case "sbyte": return (dbId, dbRef, getter) => DbCrud.GetSByte(dbId, dbRef, getter);
                case "short": return (dbId, dbRef, getter) => DbCrud.GetShort(dbId, dbRef, getter);
                case "string": return (dbId, dbRef, getter) => DbCrud.GetString(dbId, dbRef, getter);
                case "uint": return (dbId, dbRef, getter) => DbCrud.GetUInt(dbId, dbRef, getter);
                case "ulong": return (dbId, dbRef, getter) => DbCrud.GetULong(dbId, dbRef, getter);
                case "ushort": return (dbId, dbRef, getter) => DbCrud.GetUShort(dbId, dbRef, getter);
                case "byte[]": return (dbId, dbRef, getter) => DbCrud.GetBinary(dbId, dbRef, getter);
                case "reference":
                    return (dbId, dbRef, getter) => 
                    {
                        var m = DbCrud.GetDb<Starcounter.Internal.Metadata.MotherOfAllLayouts>(dbId, dbRef, getter);

                        if (m == null)
                        {
                            return null;
                        }

                        return m.GetObjectNo();
                    };
                default: throw new NotImplementedException("The data type [" + dataTypeName + "] is not supported.");
            }
        }
    }
}