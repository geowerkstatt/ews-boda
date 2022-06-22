using Microsoft.EntityFrameworkCore;

namespace EWS;

internal static class ContextFactory
{
    /// <summary>
    /// Creates an instance of <see cref="EwsContext"/> with a database. The database is seeded with data.
    /// </summary>
    /// <returns>The initialized <see cref="EwsContext"/>.</returns>
    public static EwsContext CreateContext()
    {
        var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        return context;
    }
}
