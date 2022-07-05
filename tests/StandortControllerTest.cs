using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class StandortControllerTest
{
    private EwsContext context;

    [TestInitialize]
    public void TestInitialize() => context = ContextFactory.CreateContext();

    [TestCleanup]
    public void TestCleanup() => context.Dispose();

    [TestMethod]
    public async Task GetAllAsync()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync().ConfigureAwait(false);

        Assert.AreEqual(6000, standorte.Count());

        var standortId = 30206;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Carolyn Lehner", standortToTest.AfuUser);
        Assert.AreEqual("Slovenia", standortToTest.Bemerkung);
        Assert.AreEqual("Ergonomic Fresh Shirt", standortToTest.Bezeichnung);
        Assert.AreEqual(true, standortToTest.FreigabeAfu);
        Assert.AreEqual(2575, standortToTest.Gemeinde);
        Assert.AreEqual("iwbyrzqsabb8pd8ahyd2izkurkxu9xt5q60jndil", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Carolyn_Lehner5", standortToTest.UserErstellung);
        Assert.AreEqual("Kennith.Pollich", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 3, 6).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 8, 6).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortGrundbuchnummer()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, "vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu").ConfigureAwait(false);

        Assert.AreEqual(1, standorte.Count());
        var standortToTest = standorte.Single();

        Assert.AreEqual("Elijah Schmeler", standortToTest.AfuUser);
        Assert.AreEqual("Ghana", standortToTest.Bemerkung);
        Assert.AreEqual("Incredible Frozen Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2568, standortToTest.Gemeinde);
        Assert.AreEqual("vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu", standortToTest.GrundbuchNr);
        Assert.AreEqual(2, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Elijah_Schmeler31", standortToTest.UserErstellung);
        Assert.AreEqual("Josefa_Effertz", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 2, 12).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 10, 1).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnung()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, null, "Unbranded Fresh Fish").ConfigureAwait(false);

        Assert.AreEqual(2, standorte.Count());
        var standortId = 35816;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Beverly Zulauf", standortToTest.AfuUser);
        Assert.AreEqual("Denmark", standortToTest.Bemerkung);
        Assert.AreEqual("Unbranded Fresh Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2538, standortToTest.Gemeinde);
        Assert.AreEqual("2ir0g1jdx6vfw8gpt27vr9keunmrxty6xwea94ce", standortToTest.GrundbuchNr);
        Assert.AreEqual(2, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Beverly.Zulauf", standortToTest.UserErstellung);
        Assert.AreEqual("Dusty51", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 11, 5).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 3, 16).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 4, 18).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByErstellungsdatum()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, null, null, new DateTime(2021, 11, 15)).ConfigureAwait(false);

        Assert.AreEqual(18, standorte.Count());
        var standortId = 33836;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Mamie Gutmann", standortToTest.AfuUser);
        Assert.AreEqual("Namibia", standortToTest.Bemerkung);
        Assert.AreEqual("Sleek Frozen Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2551, standortToTest.Gemeinde);
        Assert.AreEqual("rcf2cbp8t8b4amsm2ght9xqh8o3kag52kt5959ag", standortToTest.GrundbuchNr);
        Assert.AreEqual(2, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Mamie_Gutmann", standortToTest.UserErstellung);
        Assert.AreEqual("Ernestine21", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 3, 29, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 11, 15, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 7, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByMutationsdatum()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, null, null, new DateTime(2021, 11, 3)).ConfigureAwait(false);

        Assert.AreEqual(17, standorte.Count());
        var standortId = 34950;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Penny Lindgren", standortToTest.AfuUser);
        Assert.AreEqual("Honduras", standortToTest.Bemerkung);
        Assert.AreEqual("Sleek Soft Shirt", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2541, standortToTest.Gemeinde);
        Assert.AreEqual("ahbv40f1ez1eys4ndpozrpz6eyij7ffurlohnncs", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Penny_Lindgren79", standortToTest.UserErstellung);
        Assert.AreEqual("Marjorie40", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 6, 14, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 11, 3, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 5, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetBySeveralParameters()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(2475, "wj7qafzqpk7xh0zkt6px3ujisxqqwxbloxeiljz3", "Refined Concrete Tuna").ConfigureAwait(false);

        Assert.AreEqual(1, standorte.Count());
        var standortToTest = standorte.Single();

        Assert.AreEqual("Warren Hoeger", standortToTest.AfuUser);
        Assert.AreEqual("Saint Lucia", standortToTest.Bemerkung);
        Assert.AreEqual("Refined Concrete Tuna", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2475, standortToTest.Gemeinde);
        Assert.AreEqual("wj7qafzqpk7xh0zkt6px3ujisxqqwxbloxeiljz3", standortToTest.GrundbuchNr);
        Assert.AreEqual(0, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Warren54", standortToTest.UserErstellung);
        Assert.AreEqual("Abdullah61", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 8, 31).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 8, 1).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 1, 12).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetWithEmptyStrings()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, "", "", null, null).ConfigureAwait(false);

        Assert.AreEqual(6000, standorte.Count());

        var standortId = 30206;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Carolyn Lehner", standortToTest.AfuUser);
        Assert.AreEqual("Slovenia", standortToTest.Bemerkung);
        Assert.AreEqual("Ergonomic Fresh Shirt", standortToTest.Bezeichnung);
        Assert.AreEqual(true, standortToTest.FreigabeAfu);
        Assert.AreEqual(2575, standortToTest.Gemeinde);
        Assert.AreEqual("iwbyrzqsabb8pd8ahyd2izkurkxu9xt5q60jndil", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Carolyn_Lehner5", standortToTest.UserErstellung);
        Assert.AreEqual("Kennith.Pollich", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 3, 6).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 8, 6).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }
}
