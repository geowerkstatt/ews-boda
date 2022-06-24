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
        Assert.AreEqual(0, standortToTest.Bohrungen.Count);
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
        Assert.AreEqual(3, standortToTest.Bohrungen.Count);
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
        var standortToTest = standorte.Last();

        Assert.AreEqual("Beverly Zulauf", standortToTest.AfuUser);
        Assert.AreEqual("Denmark", standortToTest.Bemerkung);
        Assert.AreEqual("Unbranded Fresh Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2538, standortToTest.Gemeinde);
        Assert.AreEqual("2ir0g1jdx6vfw8gpt27vr9keunmrxty6xwea94ce", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
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

        Assert.AreEqual(17, standorte.Count());
        var standortId = 32529;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Paul Macejkovic", standortToTest.AfuUser);
        Assert.AreEqual("Nicaragua", standortToTest.Bemerkung);
        Assert.AreEqual("Handcrafted Cotton Keyboard", standortToTest.Bezeichnung);
        Assert.AreEqual(true, standortToTest.FreigabeAfu);
        Assert.AreEqual(2531, standortToTest.Gemeinde);
        Assert.AreEqual("kqqcdebqud2qy8pyekwqm8whm6tzi2ieuz0v1hdd", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Paul_Macejkovic", standortToTest.UserErstellung);
        Assert.AreEqual("Kathlyn_Hilll31", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 1, 23).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 11, 14).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 8, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByMutationsdatum()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, null, null, new DateTime(2021, 11, 3)).ConfigureAwait(false);

        Assert.AreEqual(16, standorte.Count());
        var standortId = 34640;
        var standortToTest = standorte.Single(b => b.Id == standortId);

        Assert.AreEqual("Jesus Mante", standortToTest.AfuUser);
        Assert.AreEqual("Cuba", standortToTest.Bemerkung);
        Assert.AreEqual("Awesome Fresh Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(true, standortToTest.FreigabeAfu);
        Assert.AreEqual(2550, standortToTest.Gemeinde);
        Assert.AreEqual("dlsitmex40f3re3gbbdmd6wkqivvf6z1iinhja9d", standortToTest.GrundbuchNr);
        Assert.AreEqual(1, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Jesus_Mante50", standortToTest.UserErstellung);
        Assert.AreEqual("Clint14", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 12, 20).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 11, 2).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 5, 25).Date, standortToTest.Mutationsdatum!.Value.Date);
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
        Assert.AreEqual(3, standortToTest.Bohrungen.Count);
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
        Assert.AreEqual(0, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Carolyn_Lehner5", standortToTest.UserErstellung);
        Assert.AreEqual("Kennith.Pollich", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 3, 6).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 8, 6).Date, standortToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }
}
