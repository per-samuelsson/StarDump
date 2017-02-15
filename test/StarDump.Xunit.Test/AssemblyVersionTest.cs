using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace StarDump.Xunit.Test
{
    public class AssemblyVersionTest
    {
        /// <summary>
        /// Using SemVer, MAJOR.MINOR.PATCH, as version number
        /// </summary>
        [Fact]
        public void Test_ApplicationVersion()
        {
            Assert.IsType<string>(StarDump.Core.Assembly.Version);

            string[] parts = StarDump.Core.Assembly.Version.Split('.');
            Assert.True(parts.Length == 3, "There are three parts in a SemVer 2.0 version number.");

            for(int i = 0; i < 3; i++)
            {
                Assert.True(parts[i].Length > 0, "Each part in the SemVer 2.0 version number should be set to at least one character.");
            }
        }

        /// <summary>
        /// Using SemVer, MAJOR.MINOR.PATCH, as version number
        /// </summary>
        [Fact]
        public void Test_FormatVersion()
        {
            Assert.IsType<string>(StarDump.Core.Assembly.FormatVersion);

            string[] parts = StarDump.Core.Assembly.FormatVersion.Split('.');
            Assert.True(parts.Length == 3, "There are three parts in a SemVer 2.0 version number.");

            for (int i = 0; i < 3; i++)
            {
                Assert.True(parts[i].Length > 0, "Each part in the SemVer 2.0 version number should be set to at least one character.");
            }
        }

        /// <summary>
        /// ApplicationVersionMessage is built from StarDump.Core.Assembly.Version
        /// </summary>
        [Fact]
        public void Test_ApplicationVersionMessage()
        {
            Assert.True(StarDump.Core.Assembly.ApplicationVersionMessage().Contains(StarDump.Core.Assembly.Version),
                "ApplicationVersionMessage is built from StarDump.Core.Assembly.Version");
        }

        /// <summary>
        /// FormatVersionMessage is built from StarDump.Core.Assembly.FormatVersion
        /// </summary>
        [Fact]
        public void Test_FormatVersionMessage()
        {
            Assert.True(StarDump.Core.Assembly.FormatVersionMessage().Contains(StarDump.Core.Assembly.FormatVersion),
                "FormatVersionMessage is built from StarDump.Core.Assembly.FormatVersion");
        }
    }
}
