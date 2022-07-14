using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EWS;

[TestClass]
public sealed class Initialize
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        using var context = ContextFactory.CreateContext();
        context.Database.Migrate();

        // Only seed if database is empty
        if (!context.Standorte.Any()) context.SeedData();
    }
}
