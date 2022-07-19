using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS;

internal static class ContextFactory
{
    public static string ConnectionString { get; } = "Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews";

    /// <summary>
    /// Creates an instance of <see cref="EwsContext"/> with a database. The database is seeded with data.
    /// </summary>
    /// <returns>The initialized <see cref="EwsContext"/>.</returns>
    public static EwsContext CreateContext()
    {
        return new EwsContext(
            new DbContextOptionsBuilder<EwsContext>().UseNpgsql(
                ConnectionString,
                option => option.UseNetTopologySuite().UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)).Options,
            new HttpContextAccessor() { HttpContext = new DefaultHttpContext() });
    }
}
