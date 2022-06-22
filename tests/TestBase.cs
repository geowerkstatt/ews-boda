using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EWS
{
    public abstract class TestBase
    {
        protected TestBase()
        {
            using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
                .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
            var controller = new BohrungController(context);
            if (!context.Standorte.Any())
            {
                context.SeedData();
            }
        }
    }
}
