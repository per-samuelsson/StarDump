using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StarDump.Xunit.Test
{
    /// <summary>
    /// Class which tests data conversions between Starcounter types and Sqlite types, 
    /// <see cref="Common.SqlHelper.ConvertFromStarcounterToSqlite(string, object)"/> and
    /// <see cref="Common.SqlHelper.ConvertFromSqliteToStarcounter(string, object)"/>.
    /// </summary>
    public class ConvertingDataTypesPositiveTest
    {
        private Common.SqlHelper sqlHelper = new Common.SqlHelper();

        private bool StarcounterConversionEqual<T>(string dataTypeName, T starcounterOriginalValue) where T : IEquatable<T>
        {
            // Convert to Sqlite value
            object sqlValue = sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, starcounterOriginalValue);

            // Convert back to Starcounter value
            object starcounterValue = sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, sqlValue);

            return starcounterOriginalValue.Equals(starcounterValue);
        }

        private bool SqliteConversionEqual<T>(string dataTypeName, T sqliteOriginalValue) where T : IEquatable<T>
        {
            // Convert to Starcounter value
            object starcounterValue = sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, sqliteOriginalValue);

            // Convert back to Sqlite value
            object sqlValue = sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, starcounterValue);

            return sqliteOriginalValue.Equals(sqlValue);
        }

        #region Tests
        /// <summary>
        /// Test for when converting a value = null which should always return null indepentently of data type
        /// </summary>
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
                bool isEqual = StarcounterConversionEqual<bool>(dataTypeName, true);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<bool>(dataTypeName, false);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<int>(dataTypeName, 1);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<int>(dataTypeName, 0);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_decimalDataTypeConversion()
        {
            string dataTypeName = "decimal";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<decimal>(dataTypeName, decimal.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<decimal>(dataTypeName, decimal.One);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<decimal>(dataTypeName, decimal.Zero);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<decimal>(dataTypeName, decimal.MinusOne);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<decimal>(dataTypeName, decimal.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, decimal.MaxValue.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, decimal.One.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, decimal.Zero.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, decimal.MinusOne.ToString());
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, decimal.MinValue.ToString());
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_DateTimeDataTypeConversion()
        {
            string dataTypeName = "DateTime";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<DateTime>(dataTypeName, DateTime.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<DateTime>(dataTypeName, DateTime.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, DateTime.MaxValue.Ticks);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, DateTime.MinValue.Ticks);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_doubleDataTypeConversion()
        {
            string dataTypeName = "double";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<double>(dataTypeName, double.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<double>(dataTypeName, double.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<double>(dataTypeName, double.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, double.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, double.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, double.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_floatDataTypeConversion()
        {
            string dataTypeName = "float";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<float>(dataTypeName, float.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<float>(dataTypeName, float.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<float>(dataTypeName, float.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, float.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, float.Epsilon);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<double>(dataTypeName, float.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_stringDataTypeConversion()
        {
            string dataTypeName = "string";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<string>(dataTypeName, "StarDump test string");
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<string>(dataTypeName, "");
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, "StarDump test string");
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<string>(dataTypeName, "");
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_byteDataTypeConversion()
        {
            string dataTypeName = "byte";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<byte>(dataTypeName, byte.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<byte>(dataTypeName, byte.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, byte.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, byte.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_charDataTypeConversion()
        {
            string dataTypeName = "char";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<char>(dataTypeName, char.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<char>(dataTypeName, char.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, char.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, char.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_intDataTypeConversion()
        {
            string dataTypeName = "int";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<int>(dataTypeName, int.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<int>(dataTypeName, int.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, int.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, int.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_longDataTypeConversion()
        {
            string dataTypeName = "long";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<long>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<long>(dataTypeName, long.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, long.MinValue);
                Assert.True(isEqual);
            }
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

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<short>(dataTypeName, short.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<short>(dataTypeName, short.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, short.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, short.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_uintDataTypeConversion()
        {
            string dataTypeName = "uint";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<uint>(dataTypeName, uint.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<uint>(dataTypeName, uint.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, uint.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, uint.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_ushortDataTypeConversion()
        {
            string dataTypeName = "ushort";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<ushort>(dataTypeName, ushort.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<ushort>(dataTypeName, ushort.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, ushort.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, ushort.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_referenceDataTypeConversion()
        {
            string dataTypeName = "reference";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<ulong>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<ulong>(dataTypeName, ulong.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, (long)ulong.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_ulongDataTypeConversion()
        {
            string dataTypeName = "ulong";

            // From Starcounter To Sqlite and back
            {
                bool isEqual = StarcounterConversionEqual<ulong>(dataTypeName, ulong.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = StarcounterConversionEqual<ulong>(dataTypeName, ulong.MinValue);
                Assert.True(isEqual);
            }

            // From Sqlite To Starcounter and back
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, long.MaxValue);
                Assert.True(isEqual);
            }
            {
                bool isEqual = SqliteConversionEqual<long>(dataTypeName, long.MinValue);
                Assert.True(isEqual);
            }
        }

        [Fact]
        public void Test_byteArrayDataTypeConversion()
        {
            string dataTypeName = "byte[]";

            Assert.Throws(typeof(NotImplementedException), () => { return sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, 0); });
            Assert.Throws(typeof(NotImplementedException), () => { return sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, 0); });
        }

        /// <summary>
        /// Testing an unknown data type i.e. does not exist in switch-case statement
        /// </summary>
        [Fact]
        public void Test_unknownDataTypeConversion()
        {
            string dataTypeName = "unknown";

            Assert.Throws(typeof(NotImplementedException), () => { return sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, 0); });
            Assert.Throws(typeof(NotImplementedException), () => { return sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, 0); });
        }
        #endregion
    }
}
