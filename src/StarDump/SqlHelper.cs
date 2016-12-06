using System;
using System.Text;
using Starcounter;

namespace StarDump
{
    public class SqlHelper
    {
        public string GenerateCreateMetadataTables()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("CREATE TABLE `Starcounter.Metadata.Table` (`Id` INTEGER NOT NULL, `Name` TEXT NOT NULL, `ParentId` INTEGER, PRIMARY KEY(`Id`));");
            sql.Append("CREATE TABLE `Starcounter.Metadata.Column` (`Id` INTEGER NOT NULL, `TableId` INTEGER NOT NULL, `Name` TEXT NOT NULL, `DataType` TEXT NOT NULL, `ReferenceType` TEXT NULL, `Nullable` INTEGER NOT NULL, `Inherited` INTEGER NOT NULL, PRIMARY KEY(`Id`));");

            return sql.ToString();
        }

        public string GenerateInsertMetadataTable(Starcounter.Metadata.RawView table)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO `Starcounter.Metadata.Table` (`Id`, `Name`, `ParentId`) VALUES (");
            sql.Append((long)table.GetObjectNo()).Append(", '").Append(table.FullName).Append("', ");

            if (table.Inherits == null)
            {
                sql.Append("NULL");
            }
            else
            {
                sql.Append((long)table.Inherits.GetObjectNo());
            }

            sql.Append(");");

            return sql.ToString();
        }

        public string GenerateInsertMetadataColumns(Starcounter.Metadata.RawView table, Starcounter.Metadata.Column[] columns)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO `Starcounter.Metadata.Column` (`Id`, `TableId`, `Name`, `DataType`, `ReferenceType`, `Nullable`, `Inherited`) VALUES ");

            for (int i = 0; i < columns.Length; i++)
            {
                Starcounter.Metadata.Column col = columns[i];

                if (i > 0)
                {
                    sql.Append(", ");
                }

                sql.Append("(").Append((long)col.GetObjectNo()).Append(", ")
                    .Append((long)table.GetObjectNo()).Append(", '").Append(col.Name).Append("', '")
                    .Append(col.DataType.Name).Append("', ");

                if (col.DataType.Name == "reference")
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

        public string GenerateCreateTable(Starcounter.Metadata.RawView table, Starcounter.Metadata.Column[] columns)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("CREATE TABLE `").Append(table.FullName).Append("` (`ObjectNo` INTEGER NOT NULL");

            foreach (var c in columns)
            {
                string type = this.GetSqlType(c.DataType.Name);

                sql.Append(", `").Append(c.Name).Append("` ").Append(type);

                if (!c.Nullable)
                {
                    sql.Append(" NOT NULL");
                }
            }

            sql.Append(", PRIMARY KEY(`ObjectNo`));");

            return sql.ToString();
        }

        public string GenerateInsertInto(string tableName, Starcounter.Metadata.Column[] columns, ResultRow row)
        {
            StringBuilder sql = new StringBuilder();

            this.GenerateInsertInto(tableName, columns, sql);
            this.GenerateInsertInto(columns, row, sql);
            sql.Append(";");

            return sql.ToString();
        }

        public string GenerateInsertInto(string tableName, Starcounter.Metadata.Column[] columns, ResultRow[] rows)
        {
            StringBuilder sql = new StringBuilder();

            this.GenerateInsertInto(tableName, columns, sql);

            for (int i = 0; i < rows.Length; i++)
            {
                ResultRow row = rows[i];

                if (i > 0)
                {
                    sql.Append(", ");
                }

                this.GenerateInsertInto(columns, row, sql);
            }

            sql.Append(";");

            return sql.ToString();
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

        protected void GenerateInsertInto(Starcounter.Metadata.Column[] columns, ResultRow row, StringBuilder sql)
        {
            sql.Append("(").Append((long)row.DbGetIdentity());

            foreach (var c in columns)
            {
                string type = this.GetSqlType(c.DataType.Name);
                object value = row[c.Name];

                sql.Append(", ");

                if (value == null)
                {
                    sql.Append("NULL");
                }
                else if (type == "TEXT")
                {
                    value = value.ToString().Replace("'", "''");
                    sql.Append("'").Append(value).Append("'");
                }
                else
                {
                    sql.Append(value);
                }
            }
        
            sql.Append(") ");
        }

        protected void GenerateInsertInto(string tableName, Starcounter.Metadata.Column[] columns, StringBuilder sql)
        {
            sql.Append("INSERT INTO `").Append(tableName).Append("` (`ObjectNo`");

            foreach (var c in columns)
            {
                sql.Append(", `").Append(c.Name).Append("`");
            }

            sql.Append(") VALUES");
        }
    }
}