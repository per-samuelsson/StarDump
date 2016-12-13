using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Starcounter;

namespace StarDump
{
    public class SqlHelper
    {
        public string GenerateCreateMetadataTables()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("CREATE TABLE `Starcounter.Metadata.Table` (`Id` INTEGER NOT NULL, `Name` TEXT NOT NULL, `ParentId` INTEGER, `RowsCount` INTEGER, PRIMARY KEY(`Id`));");
            sql.Append("CREATE TABLE `Starcounter.Metadata.Column` (`Id` INTEGER NOT NULL, `TableId` INTEGER NOT NULL, `Name` TEXT NOT NULL, `DataType` TEXT NOT NULL, `ReferenceType` TEXT NULL, `Nullable` INTEGER NOT NULL, `Inherited` INTEGER NOT NULL, PRIMARY KEY(`Id`));");

            return sql.ToString();
        }

        public string GenerateInsertMetadataTable(UnloadTable table)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO `Starcounter.Metadata.Table` (`Id`, `Name`, `ParentId`) VALUES (");
            sql.Append((long)table.ObjectId).Append(", '").Append(table.FullName).Append("', ");

            if (table.RawView.Inherits == null)
            {
                sql.Append("NULL");
            }
            else
            {
                sql.Append((long)table.RawView.Inherits.GetObjectNo());
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

        public string GenerateInsertInto(UnloadTable table, UnloadRow row)
        {
            StringBuilder sql = new StringBuilder();

            this.GenerateInsertIntoDefinition(table, sql);
            this.GenerateInsertIntoValues(table.Columns, row, sql);
            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateInsertInto(UnloadTable table, List<UnloadRow> rows)
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

                case "decimal":
                case "double":
                case "float":
                    return "REAL";
                
                case "string":
                    return "TEXT";

                case "byte[]":
                    throw new NotImplementedException("Type byte[] is not yet supported.");

                default: 
                    throw new NotImplementedException("Type " + dataTypeName + " is not yet supported.");
            }
        }

        public object ConvertFromStarcounterToSqlite(string starcounterDataTypeName, object value)
        {
            if (value == null)
            {
                return null;
            }

            switch (starcounterDataTypeName)
            {
                case "bool": return (bool)value ? 1 : 0;
                case "byte": return (long)((byte)value);
                case "char": return (long)((char)value);
                case "DateTime": return ((DateTime)value).Ticks;
                case "decimal": return ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                case "double": return (double)value;
                case "float": return (double)((float)value);
                case "int": return (long)((int)value);
                case "long": return (long)value;
                case "sbyte": return (long)((sbyte)value);
                case "short": return (long)((short)value);
                case "string?":
                case "string": return value as string;
                case "uint": return (long)((uint)value);
                case "ulong": return (long)((ulong)value);
                case "ushort": return (long)((ushort)value);
                
                case "bool?": return ((bool?)value).Value ? 1 : 0;
                case "byte?": return (long)(value as byte?).Value;
                case "char?": return (long)(value as char?).Value;
                case "DateTime?": return (value as DateTime?).Value.Ticks;
                case "decimal?": return decimal.Parse((string)value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat) as decimal?;
                case "double?": return (value as double?).Value;
                case "float?": return (double)(value as float?).Value;
                case "int?": return (long)(value as int?).Value;
                case "long?": return (value as long?).Value;
                case "sbyte?": return (long)(value as sbyte?).Value;
                case "short?": return (long)(value as short?).Value;
                case "uint?": return (long)(value as uint?).Value;
                case "reference":
                case "reference?":
                case "ulong?": return (long)(value as ulong?).Value;
                case "ushort?": return (long)(value as ushort?).Value;

                case "byte[]":
                default: throw new NotImplementedException("The data type [" + starcounterDataTypeName + "] is not supported.");
            }
        }

        public object ConvertFromSqliteToStarcounter(string starcounterDataTypeName, object value)
        {
            if (value == null || System.DBNull.Value.Equals(value))
            {
                return null;
            }

            switch (starcounterDataTypeName)
            {
                case "bool": return (long)value == 1;
                case "byte": return (byte)((long)value);
                case "char": return (char)((long)value);
                case "DateTime": return new DateTime((long)value);
                case "decimal": return decimal.Parse((string)value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                case "double": return (double)value;
                case "float": return (float)((double)value);
                case "int": return (int)((long)value);
                case "long": return (long)value;
                case "sbyte": return (sbyte)((long)value);
                case "short": return (short)((long)value);
                case "string?":
                case "string": return value as string;
                case "uint": return (uint)((long)value);
                case "ulong": return (ulong)((long)value);
                case "ushort": return (ushort)((long)value);
                case "bool?": return ((long)value == 1) as bool?;
                case "byte?": return (byte)((long)value) as byte?;
                case "char?": return (char)((long)value) as char?;
                case "DateTime?": return new DateTime((long)value) as DateTime?;
                case "decimal?": return decimal.Parse((string)value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat) as decimal?;
                case "double?": return (double)value as double?;
                case "float?": return (float)((double)value) as float?;
                case "int?": return (int)((long)value) as int?;
                case "long?": return (long)value as long?;
                case "sbyte?": return (sbyte)((long)value) as sbyte?;
                case "short?": return (short)((long)value) as short?;
                case "uint?": return (uint)((long)value) as uint?;
                case "reference":
                case "reference?":
                case "ulong?": return (ulong)((long)value) as ulong?;
                case "ushort?": return (ushort)((long)value) as ushort?;

                case "byte[]":
                default: throw new NotImplementedException("The data type [" + starcounterDataTypeName + "] is not supported.");
            }
        }

        public void SetupSqliteConnection(SqliteConnection cn)
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

        public void ExecuteNonQuery(string sql, SqliteConnection cn)
        {
            SqliteCommand cmd = new SqliteCommand(sql, cn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Unable to execute SQL: " + sql, ex);
            }
        }

        public void GenerateInsertIntoValues(List<UnloadColumn> columns, List<UnloadRow> rows, StringBuilder sql)
        {
            bool first = true;

            foreach (UnloadRow row in rows)
            {
                if (!first)
                {
                    sql.Append(", ");
                }

                first = false;
                this.GenerateInsertIntoValues(columns, row, sql);
            }
        }

        public void GenerateInsertIntoValues(List<UnloadColumn> columns, UnloadRow row, StringBuilder sql)
        {
            sql.Append("(").Append((long)row.DbGetIdentity());

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