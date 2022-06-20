using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class BohrungControllerTest
{
    [TestMethod]
    public async Task GetAsync()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var result = await controller.GetAsync().ConfigureAwait(false);
        var bohrungen = result.Value;
        Assert.AreEqual(8000, bohrungen?.Count());
        Assert.AreEqual("Reverse-engineered web-enabled emulation", bohrungen?.First().Bemerkung);
        Assert.AreEqual(27330, bohrungen?.Last().DurchmesserBohrloch);
        Assert.AreEqual(new Point(2603642, 1240821), bohrungen?.First().Geometrie);
        Assert.AreEqual(new DateTime(2022, 4, 29, 0, 0, 0), bohrungen?.Last().Datum);
    }
}
