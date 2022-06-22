using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        Assert.AreEqual(6000, standorte?.Count());

        var standortId = 30206;
        var standortToTest = standorte?.Single(b => b.Id == standortId);

        Assert.AreEqual("Carolyn Lehner", standortToTest?.AfuUser);
        Assert.AreEqual("Slovenia", standortToTest?.Bemerkung);
        Assert.AreEqual("Ergonomic Fresh Shirt", standortToTest?.Bezeichnung);
        Assert.AreEqual(true, standortToTest?.FreigabeAfu);
        Assert.AreEqual(2575, standortToTest?.Gemeinde);
        Assert.AreEqual("iwbyrzqsabb8pd8ahyd2izkurkxu9xt5q60jndil", standortToTest?.GrundbuchNr);
        Assert.AreEqual("Carolyn_Lehner5", standortToTest?.UserErstellung);
        Assert.AreEqual("Kennith.Pollich", standortToTest?.UserMutation);
    }

    [TestMethod]
    public async Task GetByStandortGrundbuchnummer()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, "vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu").ConfigureAwait(false);

        Assert.AreEqual(1, standorte?.Count());
        var standortToTest = standorte?.Single();

        Assert.AreEqual("Elijah Schmeler", standortToTest?.AfuUser);
        Assert.AreEqual("Ghana", standortToTest?.Bemerkung);
        Assert.AreEqual("Incredible Frozen Fish", standortToTest?.Bezeichnung);
        Assert.AreEqual(false, standortToTest?.FreigabeAfu);
        Assert.AreEqual(2568, standortToTest?.Gemeinde);
        Assert.AreEqual("vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu", standortToTest?.GrundbuchNr);
        Assert.AreEqual("Elijah_Schmeler31", standortToTest?.UserErstellung);
        Assert.AreEqual("Josefa_Effertz", standortToTest?.UserMutation);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnung()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(null, null, "Unbranded Fresh Fish").ConfigureAwait(false);

        Assert.AreEqual(2, standorte?.Count());
        var standortToTest = standorte?.Last();

        Assert.AreEqual("Beverly Zulauf", standortToTest?.AfuUser);
        Assert.AreEqual("Denmark", standortToTest?.Bemerkung);
        Assert.AreEqual("Unbranded Fresh Fish", standortToTest?.Bezeichnung);
        Assert.AreEqual(false, standortToTest?.FreigabeAfu);
        Assert.AreEqual(2538, standortToTest?.Gemeinde);
        Assert.AreEqual("2ir0g1jdx6vfw8gpt27vr9keunmrxty6xwea94ce", standortToTest?.GrundbuchNr);
        Assert.AreEqual("Beverly.Zulauf", standortToTest?.UserErstellung);
        Assert.AreEqual("Dusty51", standortToTest?.UserMutation);
    }

    [TestMethod]
    public async Task GetBySeveralParameters()
    {
        var controller = new StandortController(context);
        var standorte = await controller.GetAsync(2475, "wj7qafzqpk7xh0zkt6px3ujisxqqwxbloxeiljz3", "Refined Concrete Tuna").ConfigureAwait(false);

        Assert.AreEqual(1, standorte?.Count());
        var standortToTest = standorte?.Single();

        Assert.AreEqual("Warren Hoeger", standortToTest?.AfuUser);
        Assert.AreEqual("Saint Lucia", standortToTest?.Bemerkung);
        Assert.AreEqual("Refined Concrete Tuna", standortToTest?.Bezeichnung);
        Assert.AreEqual(false, standortToTest?.FreigabeAfu);
        Assert.AreEqual(2475, standortToTest?.Gemeinde);
        Assert.AreEqual("wj7qafzqpk7xh0zkt6px3ujisxqqwxbloxeiljz3", standortToTest?.GrundbuchNr);
        Assert.AreEqual("Warren54", standortToTest?.UserErstellung);
        Assert.AreEqual("Abdullah61", standortToTest?.UserMutation);
    }
}
