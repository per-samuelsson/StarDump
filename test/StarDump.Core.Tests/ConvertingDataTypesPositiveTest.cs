using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StarDump.Core.Tests
{
    /// <summary>
    /// Class which tests data conversions between Starcounter types and Sqlite types, 
    /// <see cref="Common.SqlHelper.ConvertFromStarcounterToSqlite(string, object)"/> and
    /// <see cref="Common.SqlHelper.ConvertFromSqliteToStarcounter(string, object)"/>.
    /// </summary>
    public class ConvertingDataTypesPositiveTest
    {
        private Common.SqlHelper sqlHelper = new Common.SqlHelper();

        private bool StarcounterConvertionEqual<T>(string dataTypeName, T starcounterOriginalValue) where T : IEquatable<T>
        {
            // Convert to Sqlite value
            object sqlValue = sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, starcounterOriginalValue);

            // Convert back to Starcounter value
            object starcounterValue = sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, sqlValue);

            return starcounterOriginalValue.Equals(starcounterValue);
        }

        private bool SqliteConvertionEqual<T>(string dataTypeName, T sqliteOriginalValue) where T : IEquatable<T>
        {
            // Convert to Starcounter value
            object starcounterValue = sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, sqliteOriginalValue);

            // Convert back to Sqlite value
            object sqlValue = sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, starcounterValue);

            return sqliteOriginalValue.Equals(sqlValue);
        }

        #region Tests
        [Fact]
        public void Test_NullValue()
        {
            // Return value should be null is value argument is null
            {
                object isNull = sqlHelper.ConvertFromStarcounterToSqlite(null, value: null);
                Assert.Null(isNull);
            }
            {
                object isNull = sqlHelper.ConvertFromSqliteToStarcounter(null, value: null);
                Assert.Null(isNull);
            }
        }

        [Fact]
        public void Test_boolDataTypeConversion()
        {
            string dataTypeName = "bool";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConvertionEqual<bool>(dataTypeName, true);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<bool>(dataTypeName, false);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConvertionEqual<int>(dataTypeName, 1);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<int>(dataTypeName, 0);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_decimalDataTypeConversion()
        {
            string dataTypeName = "decimal";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConvertionEqual<decimal>(dataTypeName, decimal.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<decimal>(dataTypeName, decimal.One);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<decimal>(dataTypeName, decimal.Zero);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<decimal>(dataTypeName, decimal.MinusOne);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<decimal>(dataTypeName, decimal.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConvertionEqual<string>(dataTypeName, decimal.MaxValue.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<string>(dataTypeName, decimal.One.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<string>(dataTypeName, decimal.Zero.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<string>(dataTypeName, decimal.MinusOne.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<string>(dataTypeName, decimal.MinValue.ToString());
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_DateTimeDataTypeConversion()
        {
            string dataTypeName = "DateTime";

            //TODO
        }

        [Fact]
        public void Test_doubleDataTypeConversion()
        {
            string dataTypeName = "double";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConvertionEqual<double>(dataTypeName, double.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<double>(dataTypeName, double.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<double>(dataTypeName, double.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, double.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, double.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, double.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_floatDataTypeConversion()
        {
            string dataTypeName = "float";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConvertionEqual<float>(dataTypeName, float.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<float>(dataTypeName, float.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<float>(dataTypeName, float.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, float.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, float.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<double>(dataTypeName, float.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_stringDataTypeConversion()
        {
            string dataTypeName = "string";

            //TODO
        }

        [Fact]
        public void Test_byteDataTypeConversion()
        {
            string dataTypeName = "byte";

            //TODO
        }

        [Fact]
        public void Test_charDataTypeConversion()
        {
            string dataTypeName = "char";

            //TODO
        }

        [Fact]
        public void Test_intDataTypeConversion()
        {
            string dataTypeName = "int";

            //TODO
        }

        [Fact]
        public void Test_longDataTypeConversion()
        {
            string dataTypeName = "long";

            //TODO
        }

        [Fact]
        public void Test_sbyteDataTypeConversion()
        {
            string dataTypeName = "sbyte";

            //TODO
        }

        [Fact]
        public void Test_shortDataTypeConversion()
        {
            string dataTypeName = "short";

            //TODO
        }

        [Fact]
        public void Test_uintDataTypeConversion()
        {
            string dataTypeName = "uint";

            //TODO
        }

        [Fact]
        public void Test_ushortDataTypeConversion()
        {
            string dataTypeName = "ushort";

            //TODO
        }

        [Fact]
        public void Test_referenceDataTypeConversion()
        {
            string dataTypeName = "reference";

            //TODO
        }

        [Fact]
        public void Test_ulongDataTypeConversion()
        {
            string dataTypeName = "ulong";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConvertionEqual<ulong>(dataTypeName, ulong.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConvertionEqual<ulong>(dataTypeName, ulong.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConvertionEqual<long>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConvertionEqual<long>(dataTypeName, long.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_byteArrayDataTypeConversion()
        {
            string dataTypeName = "byte[]";

            //TODO
        }
        #endregion
    }
}
