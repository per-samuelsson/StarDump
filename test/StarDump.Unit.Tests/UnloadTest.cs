using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Starcounter.Core;

namespace StarDump.Unit.Tests
{
    // [Database]
    // public class Parent
    // {
    //     public virtual string Name { get; set; }
    // }

    public class UnloadTest
    {
        // [Fact]
        // public void Test_Unload()
        // {
        //     string[] args = new string[] { };
        //     var host = new AppHostBuilder().AddCommandLine(args).Build();
            
        //     host.Start();

        //     Db.Transact(() =>
        //     {
        //         Parent p = Db.Insert<Parent>();
        //         p.Name = "Xunit test";
        //         //new Parent() { Name = "Xunit test" };

        //         p = Db.SQL<Parent>("SELECT p FROM StarDump.Unit.Tests.Parent p").FirstOrDefault();

        //         Assert.Equal(p.Name, "Xunit test");
        //     });

        //     host.Dispose();
        // }
    }
}