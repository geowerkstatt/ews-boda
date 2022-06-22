using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class BohrungControllerTest
{
    [TestMethod]
    public async Task GetAllAsync()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var bohrungen = await controller.GetAsync().ConfigureAwait(false);

        Assert.AreEqual(8000, bohrungen?.Count());

        var bohrungId = 42764;
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
        Assert.AreEqual(new Point(2624970, 1252908), bohrungToTest?.Geometrie);
    }

    [TestMethod]
    public async Task GetByStandortGemeindeAsync()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var bohrungen = await controller.GetAsync(2450).ConfigureAwait(false);

        Assert.AreEqual(32, bohrungen?.Count());

        var bohrungId = 46038;
        var bohrungToTest = bohrungen?.Single(b => b.Id == bohrungId);
        Assert.AreEqual(null, bohrungToTest?.Bemerkung);
        Assert.AreEqual(33866, bohrungToTest?.StandortId);
        Assert.AreEqual("Refined Fresh Chips", bohrungToTest?.Bezeichnung);
        Assert.AreEqual(298, bohrungToTest?.Ablenkung);
        Assert.AreEqual(6709, bohrungToTest?.DurchmesserBohrloch);
        Assert.AreEqual("It only works when I'm Kuwait.", bohrungToTest?.QualitaetBemerkung);
        Assert.AreEqual("Bergstrom, Lindgren and Yundt", bohrungToTest?.QuelleRef);
        Assert.AreEqual("Archie_Hermann", bohrungToTest?.UserErstellung);
        Assert.AreEqual("Helena.Zboncak42", bohrungToTest?.UserMutation);
        Assert.AreEqual(18, bohrungToTest?.Qualitaet);
        Assert.AreEqual(3, bohrungToTest?.HQualitaet);
        Assert.AreEqual(9, bohrungToTest?.HAblenkung);
        Assert.AreEqual(new Point(2624511, 1252945), bohrungToTest?.Geometrie);
    }

    [TestMethod]
    public async Task GetByStandortGrundbuchnummer()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var bohrungen = await controller.GetAsync(null, "vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu").ConfigureAwait(false);

        Assert.AreEqual(3, bohrungen?.Count());

        var bohrungId = 41283;
        var bohrungToTest = bohrungen?.Single(b => b.Id == bohrungId);
        Assert.AreEqual("Function-based contextually-based orchestration", bohrungToTest?.Bemerkung);
        Assert.AreEqual(31464, bohrungToTest?.StandortId);
        Assert.AreEqual("Practical Soft Chicken", bohrungToTest?.Bezeichnung);
        Assert.AreEqual(32, bohrungToTest?.Ablenkung);
        Assert.AreEqual(543, bohrungToTest?.DurchmesserBohrloch);
        Assert.AreEqual("The box this comes in is 3 yard by 6 yard and weights 19 pound!!!", bohrungToTest?.QualitaetBemerkung);
        Assert.AreEqual("Parker, Bartell and Kris", bohrungToTest?.QuelleRef);
        Assert.AreEqual("Judith.Lockman", bohrungToTest?.UserErstellung);
        Assert.AreEqual("Hazle.Reinger96", bohrungToTest?.UserMutation);
        Assert.AreEqual(41, bohrungToTest?.Qualitaet);
        Assert.AreEqual(3, bohrungToTest?.HQualitaet);
        Assert.AreEqual(9, bohrungToTest?.HAblenkung);
        Assert.AreEqual(new Point(2639243, 1250178), bohrungToTest?.Geometrie);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnung()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var bohrungen = await controller.GetAsync(null, null, "Unbranded Fresh Fish").ConfigureAwait(false);

        Assert.AreEqual(5, bohrungen?.Count());

        var bohrungId = 43997;
        var bohrungToTest = bohrungen?.Single(b => b.Id == bohrungId);
        Assert.AreEqual("Sharable intangible paradigm", bohrungToTest?.Bemerkung);
        Assert.AreEqual(33052, bohrungToTest?.StandortId);
        Assert.AreEqual("Rustic Concrete Bike", bohrungToTest?.Bezeichnung);
        Assert.AreEqual(53, bohrungToTest?.Ablenkung);
        Assert.AreEqual(10687, bohrungToTest?.DurchmesserBohrloch);
        Assert.AreEqual("this product is standard.", bohrungToTest?.QualitaetBemerkung);
        Assert.AreEqual("Kiehn Inc", bohrungToTest?.QuelleRef);
        Assert.AreEqual("Doug.Blick", bohrungToTest?.UserErstellung);
        Assert.AreEqual("Fannie20", bohrungToTest?.UserMutation);
        Assert.AreEqual(152, bohrungToTest?.Qualitaet);
        Assert.AreEqual(3, bohrungToTest?.HQualitaet);
        Assert.AreEqual(9, bohrungToTest?.HAblenkung);
        Assert.AreEqual(new Point(2628263, 1228926), bohrungToTest?.Geometrie);
    }

    [TestMethod]
    public async Task GetBySeveralParameters()
    {
        using var context = new EwsContext(new DbContextOptionsBuilder<EwsContext>()
            .UseNpgsql("Host=localhost; Username=postgres;Password=VINTAGEMAGIC;Database=ews", option => option.UseNetTopologySuite()).Options);
        var controller = new BohrungController(context);
        var bohrungen = await controller.GetAsync(2475, "wj7qafzqpk7xh0zkt6px3ujisxqqwxbloxeiljz3", "Refined Concrete Tuna").ConfigureAwait(false);

        Assert.AreEqual(3, bohrungen?.Count());

        var bohrungId = 40070;
        var bohrungToTest = bohrungen?.Single(b => b.Id == bohrungId);
        Assert.AreEqual(null, bohrungToTest?.Bemerkung);
        Assert.AreEqual(32225, bohrungToTest?.StandortId);
        Assert.AreEqual("Incredible Wooden Computer", bohrungToTest?.Bezeichnung);
        Assert.AreEqual(97, bohrungToTest?.Ablenkung);
        Assert.AreEqual(3040, bohrungToTest?.DurchmesserBohrloch);
        Assert.AreEqual("talk about contempt!", bohrungToTest?.QualitaetBemerkung);
        Assert.AreEqual("Nolan LLC", bohrungToTest?.QuelleRef);
        Assert.AreEqual("Laurence.McCullough", bohrungToTest?.UserErstellung);
        Assert.AreEqual("Izabella75", bohrungToTest?.UserMutation);
        Assert.AreEqual(85, bohrungToTest?.Qualitaet);
        Assert.AreEqual(3, bohrungToTest?.HQualitaet);
        Assert.AreEqual(9, bohrungToTest?.HAblenkung);
        Assert.AreEqual(new Point(2598825, 1254459), bohrungToTest?.Geometrie);
    }
}
