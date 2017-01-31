using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Common;

namespace StarDump.Common
{
    public class SqlHelper
    {
        public string GenerateCreateMetadataTables()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("CREATE TABLE `Starcounter.Metadata.Table` (`Id` INTEGER NOT NULL, `Name` TEXT NOT NULL, `ParentId` INTEGER, `RowsCount` INTEGER, PRIMARY KEY(`Id`)); ");
            sql.Append("CREATE TABLE `Starcounter.Metadata.Column` (`Id` INTEGER NOT NULL, `TableId` INTEGER NOT NULL, `Name` TEXT NOT NULL, `DataType` TEXT NOT NULL, `ReferenceType` TEXT NULL, `Nullable` INTEGER NOT NULL, `Inherited` INTEGER NOT NULL, PRIMARY KEY(`Id`)); ");
            sql.Append("CREATE TABLE `Starcounter.Metadata.Info` (`Id` INTEGER NOT NULL, `Name` TEXT NOT NULL, `Value` TEXT, PRIMARY KEY(`Id`)); ");

            return sql.ToString();
        }

        public string GenerateInsertMetadataTable(UnloadTable table)
        {
            StringBuilder sql = new StringBuilder();
            ulong? parentId = table.GetParentId();

            sql.Append("INSERT INTO `Starcounter.Metadata.Table` (`Id`, `Name`, `ParentId`) VALUES (");
            sql.Append((long)table.ObjectId).Append(", '").Append(table.FullName).Append("', ");

            if (parentId == null)
            {
                sql.Append("NULL");
            }
            else
            {
                sql.Append((long)parentId);
            }

            sql.Append(");");

            return sql.ToString();
        }

        public string GenerateUpdateMetadataTableRowsCount(string tableName, ulong rowsCount)
        {
            StringBuilder sql = new StringBuilder();
            long count = (long)rowsCount;

            sql.Append("UPDATE `Starcounter.Metadata.Table` SET `RowsCount` = ").Append(count).Append(" WHERE `Name` = '").Append(tableName).Append("';");

            return sql.ToString();
        }

        public string GenerateInsertMetadataColumns(UnloadTable table)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO `Starcounter.Metadata.Column` (`Id`, `TableId`, `Name`, `DataType`, `ReferenceType`, `Nullable`, `Inherited`) VALUES ");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                UnloadColumn col = table.Columns[i];

                if (i > 0)
                {
                    sql.Append(", ");
                }

                sql.Append("(").Append((long)col.ObjectId).Append(", ")
                    .Append((long)table.ObjectId).Append(", '").Append(col.Name).Append("', '")
                    .Append(col.DataTypeName).Append("', ");

                if (col.DataTypeName == "reference")
                {
                    // TODO: insert reference type name
                    sql.Append("NULL");
                }
                else
                {
                    sql.Append("NULL");
                }

                sql.Append(", ").Append(col.Nullable ? 1 : 0).Append(", ").Append(col.Inherited ? 1 : 0).Append(")");
            }

            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateInsertMetaInfo(List<MetaInfo> infos)
        {
            bool first = true;
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO `Starcounter.Metadata.Info` (`Id`, `Name`, `Value`) VALUES ");

            foreach (MetaInfo info in infos)
            {
                if (!first)
                {
                    sql.Append(", ");
                }
                else
                {
                    first = false;
                }

                sql.Append("(").Append(info.Id).Append(", '").Append(this.EscapeSqliteString(info.Name)).Append("', '").Append(this.EscapeSqliteString(info.Value)).Append("')");
            }

            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateCreateTable(UnloadTable table)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("CREATE TABLE `").Append(table.FullName).Append("` (`ObjectNo` INTEGER NOT NULL");

            foreach (UnloadColumn c in table.Columns)
            {
                string type = this.GetSqlType(c.DataTypeName);

                sql.Append(", `").Append(c.Name).Append("` ").Append(type);

                if (!c.Nullable)
                {
                    sql.Append(" NOT NULL");
                }
            }

            sql.Append(", PRIMARY KEY(`ObjectNo`));");

            return sql.ToString();
        }

        public string GenerateInsertIntoWithParams(UnloadTable table)
        {
            StringBuilder sql = new StringBuilder();
            
            this.GenerateInsertIntoDefinition(table, sql);

            sql.Append("(@ObjectNo");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sql.Append(", @").Append(table.Columns[i].Name);
            }

            sql.Append(");");

            return sql.ToString();
        }

        public string GenerateInsertInto(UnloadTable table, DumpRow row)
        {
            StringBuilder sql = new StringBuilder();

            this.GenerateInsertIntoDefinition(table, sql);
            this.GenerateInsertIntoValues(table.Columns, row, sql);
            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateInsertInto(UnloadTable table, List<DumpRow> rows)
        {
            StringBuilder sql = new StringBuilder();

            this.GenerateInsertIntoDefinition(table, sql);
            this.GenerateInsertIntoValues(table.Columns, rows, sql);
            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateSelectFrom(string tableName, string[] columnNames)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ObjectNo");

            foreach (string c in columnNames)
            {
                sql.Append(", `").Append(c).Append("`");
            }

            sql.Append(" FROM `").Append(tableName).Append("`");

            return sql.ToString();
        }

        public string GenerateSelectTables()
        {
            return "SELECT `Id`, `Name`, `ParentId` FROM `Starcounter.Metadata.Table`";
        }

        public string GenerateSelectMetadataColumns()
        {
            return "SELECT `Id`, `TableId`, `Name`, `DataType`, `ReferenceType`, `Nullable`, `Inherited` FROM `Starcounter.Metadata.Column`";
        }

        public string GenerateSelectMetadataColumns(long tableId)
        {
            return "SELECT `Id`, `TableId`, `Name`, `DataType`, `ReferenceType`, `Nullable`, `Inherited` FROM `Starcounter.Metadata.Column` WHERE `TableId` = " + tableId;
        }

        public string GetSqlType(string dataTypeName)
        {
            switch (dataTypeName)
            {
                case "bool":
                case "byte":
                case "char":
                case "DateTime":
                case "int":
                case "long":
                case "sbyte":
                case "short":
                case "uint":
                case "ulong":
                case "ushort":
                case "reference":
                    return "INTEGER";

                case "double":
                case "float":
                    return "REAL";
                
                case "decimal":
                case "string":
                    return "TEXT";

                case "byte[]":
                    throw new NotImplementedException("Type byte[] is not yet supported.");

                default: 
                    throw new NotImplementedException("Type " + dataTypeName + " is not yet supported.");
            }
        }

        public virtual object ConvertFromStarcounterToSqlite(string starcounterDataTypeName, object value)
        {
            if (value == null)
            {
                return null;
            }

            switch (starcounterDataTypeName)
            {
                case "bool?":
                case "bool": return Convert.ToBoolean(value) ? 1 : 0;

                case "decimal?":
                case "decimal": return (Convert.ToDecimal(value)).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                case "DateTime?":
                case "DateTime": return ((DateTime)value).Ticks;

                case "double?":
                case "double": return Convert.ToDouble(value);
                case "float?":
                case "float": return Convert.ToDouble(value);

                case "string?":
                case "string": return value as string;

                case "byte?":
                case "byte":
                case "char?":
                case "char":
                case "int?":
                case "int":
                case "long?":
                case "long":
                case "sbyte?":
                case "sbyte":
                case "short?":
                case "short":
                case "uint?":
                case "uint":
                case "ushort":
                case "reference":
                case "reference?":
                case "ushort?": return Convert.ToInt64(value);

                case "ulong":
                case "ulong?": return (long)Convert.ToUInt64(value);

                case "byte[]":
                default: throw new NotImplementedException("The data type [" + starcounterDataTypeName + "] is not supported.");
            }
        }

        public virtual object ConvertFromSqliteToStarcounter(string starcounterDataTypeName, object value)
        {
            if (value == null || System.DBNull.Value.Equals(value))
            {
                return null;
            }

            switch (starcounterDataTypeName)
            {
                case "bool": return Convert.ToInt64(value) == 1;
                case "byte": return Convert.ToByte(value);
                case "char": return Convert.ToChar(value);
                case "DateTime": return new DateTime(Convert.ToInt64(value));
                case "decimal": return decimal.Parse(Convert.ToString(value), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                case "double": return Convert.ToDouble(value);
                case "float": return (float)Convert.ToDouble(value);
                case "int": return Convert.ToInt32(value);
                case "long": return Convert.ToInt64(value);
                case "sbyte": return Convert.ToSByte(value);
                case "short": return Convert.ToInt16(value);
                case "string?":
                case "string": return Convert.ToString(value);
                case "uint": return Convert.ToUInt32(value);
                case "ulong": return (ulong)Convert.ToInt64(value);
                case "ushort": return Convert.ToUInt16(value);
                case "bool?": return (Convert.ToInt64(value) == 1) as bool?;
                case "byte?": return Convert.ToByte(value) as byte?;
                case "char?": return Convert.ToChar(value) as char?;
                case "DateTime?": return new DateTime(Convert.ToInt64(value)) as DateTime?;
                case "decimal?": return decimal.Parse(Convert.ToString(value), System.Globalization.CultureInfo.InvariantCulture.NumberFormat) as decimal?;
                case "double?": return Convert.ToDouble(value) as double?;
                case "float?": return (float)(Convert.ToDouble(value)) as float?;
                case "int?": return Convert.ToInt32(value) as int?;
                case "long?": return Convert.ToInt64(value) as long?;
                case "sbyte?": return Convert.ToSByte(value) as sbyte?;
                case "short?": return Convert.ToInt16(value) as short?;
                case "uint?": return Convert.ToUInt32(value) as uint?;
                case "reference":
                case "reference?":
                case "ulong?": return (ulong?)Convert.ToInt64(value);
                case "ushort?": return Convert.ToUInt16(value) as ushort?;

                case "byte[]":
                default: throw new NotImplementedException("The data type [" + starcounterDataTypeName + "] is not supported.");
            }
        }

        public void SetupSqliteConnection(DbConnection cn)
        {
            // http://blog.quibb.org/2010/08/fast-bulk-inserts-into-sqlite/
            this.ExecuteNonQuery("PRAGMA synchronous=OFF", cn);
            this.ExecuteNonQuery("PRAGMA count_changes=OFF", cn);
            // this.ExecuteNonQuery("PRAGMA journal_mode=MEMORY", cn);
            this.ExecuteNonQuery("PRAGMA journal_mode=OFF", cn);
            this.ExecuteNonQuery("PRAGMA temp_store=MEMORY", cn);
            // this.ExecuteNonQuery("PRAGMA cache_size=-500000", cn);
            // this.ExecuteNonQuery("PRAGMA cache_spill=50", cn);
            this.ExecuteNonQuery("PRAGMA read_uncommitted=TRUE", cn);
        }

        public void ExecuteNonQuery(string sql, DbConnection cn)
        {
            DbCommand cmd = cn.CreateCommand();

            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Unable to execute SQL: " + sql, ex);
            }
        }

        public void GenerateInsertIntoValues<TColumn>(List<TColumn> columns, List<DumpRow> rows, StringBuilder sql) where TColumn : UnloadColumn
        {
            bool first = true;

            foreach (DumpRow row in rows)
            {
                if (!first)
                {
                    sql.Append(", ");
                }

                first = false;
                this.GenerateInsertIntoValues(columns, row, sql);
            }
        }

        public void GenerateInsertIntoValues<TColumn>(List<TColumn> columns, DumpRow row, StringBuilder sql) where TColumn : UnloadColumn
        {
            sql.Append("(").Append((long)row.DbObjectIdentity);

            foreach (var c in columns)
            {
                string type = this.GetSqlType(c.DataTypeName);
                object value = row[c.Name];

                value = this.ConvertFromStarcounterToSqlite(c.DataTypeName, value);
                sql.Append(", ");

                if (value == null)
                {
                    sql.Append("NULL");
                }
                else if (type == "TEXT")
                {
                    value = this.EscapeSqliteString(value.ToString());
                    sql.Append("'").Append(value).Append("'");
                }
                else if (double.PositiveInfinity.Equals(value))
                {
                    sql.Append("9e999");
                }
                else if (double.NegativeInfinity.Equals(value))
                {
                    sql.Append("-9e999");
                }
                else
                {
                    sql.Append(value);
                }
            }
        
            sql.Append(")");
        }

        public void GenerateInsertIntoDefinition(UnloadTable table, StringBuilder sql)
        {
            sql.Append("INSERT INTO `").Append(table.FullName).Append("` (`ObjectNo`");

            foreach (var c in table.Columns)
            {
                sql.Append(", `").Append(c.Name).Append("`");
            }

            sql.Append(") VALUES");
        }

        public string EscapeSqliteString(string value)
        {
            StringBuilder sb = new StringBuilder(value.Length);

            foreach (char ch in value)
            {
                switch (ch)
                {
                    case '\'':
                        sb.Append("''");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}