using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StarDump.Core.Tests
{
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

        private bool SqliteConvertionEqual<T>(string dataTypeName, T sqlOriginalValue) where T : IEquatable<T>
        {
            // Convert to Starcounter value
            object starcounterValue = sqlHelper.ConvertFromSqliteToStarcounter(dataTypeName, sqlOriginalValue);

            // Convert back to Sqlite value
            object sqlValue = sqlHelper.ConvertFromStarcounterToSqlite(dataTypeName, starcounterValue);

            return sqlOriginalValue.Equals(sqlValue);
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

    }
}
