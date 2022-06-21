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
        var bohrungen = await controller.GetAsync().ConfigureAwait(false);

        Assert.AreEqual(8000, bohrungen?.Count());

        int bohrungId = 42764;
        var bohrungToTest = bohrungen?.Single(b => b.Id == bohrungId);
        Assert.AreEqual("Enterprise-wide attitude-oriented application", bohrungToTest?.Bemerkung);
        Assert.AreEqual(34377, bohrungToTest?.StandortId);
        Assert.AreEqual("Generic Rubber Pizza", bohrungToTest?.Bezeichnung);
        Assert.AreEqual(99, bohrungToTest?.Ablenkung);
        Assert.AreEqual(17133, bohrungToTest?.DurchmesserBohrloch);
        Assert.AreEqual("I saw one of these in French Southern and Antarctic Lands and I bought one.", bohrungToTest?.QualitaetBemerkung);
        Assert.AreEqual("Walsh - Mitchell", bohrungToTest?.QuelleRef);
        Assert.AreEqual("Kristina64", bohrungToTest?.UserErstellung);
        Assert.AreEqual("Allison.Lehner77", bohrungToTest?.UserMutation);
        Assert.AreEqual(85, bohrungToTest?.Qualitaet);
        Assert.AreEqual(3, bohrungToTest?.HQualitaet);
        Assert.AreEqual(9, bohrungToTest?.HAblenkung);
        Assert.AreEqual(new DateTime(2021, 9, 29, 0, 0, 0), bohrungToTest?.Datum.RoundToSeconds());
        Assert.AreEqual(new DateTime(2022, 6, 19, 15, 49, 54), bohrungToTest?.Mutationsdatum.RoundToSeconds());
        Assert.AreEqual(new DateTime(2021, 7, 12, 9, 42, 50), bohrungToTest?.Erstellungsdatum.RoundToSeconds());
        Assert.AreEqual(new Point(2624970, 1252908), bohrungToTest?.Geometrie);
    }
}
