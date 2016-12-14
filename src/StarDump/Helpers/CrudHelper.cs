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
        protected Dictionary<string, Func<ulong, ulong, ulong, object>> methods = new Dictionary<string, Func<ulong, ulong, ulong, object>>();

        public CrudHelper(ulong dbHandle, UnloadTable table)
        {
            this.TableName = table.FullName;

            foreach (UnloadColumn c in table.Columns)
            {
                ulong getter;

                sccrud.star_crud_GetPropertyReadHandle(dbHandle, this.TableName, c.Name, out getter);
                getters.Add(c.Name, getter);
                methods.Add(c.Name, this.GetMethod(c.DataTypeName, c.Nullable));
            }
        }

        public ulong GetGetter(string columnName)
        {
            return getters[columnName];
        }

        public object GetValue(ulong dbId, ulong dbRef, string columnName)
        {
            ulong getter = getters[columnName];
            Func<ulong, ulong, ulong, object> method = this.methods[columnName];
            object value = method(dbId, dbRef, getter);

            return value;
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