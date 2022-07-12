using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class CodeControllerTest
{
    private EwsContext context;
    private CodeController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new CodeController(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task GetByCodeTypAsync()
    {
        var codeId = 303;

        // get codes with codetype 1
        var codes = await controller.GetByCodeTypAsync(1).ConfigureAwait(false);
        Assert.AreEqual(20, codes.Count());

        var codeToTest = codes.Single(b => b.Id == codeId);

        Assert.AreEqual(1, codeToTest.CodetypId);
        Assert.AreEqual("West Maraview", codeToTest.Kurztext);
        Assert.AreEqual(13911, codeToTest.Sortierung);
        Assert.AreEqual("Handmade Rubber Bike", codeToTest.Text);
        Assert.AreEqual("Dexter26", codeToTest.UserErstellung);
        Assert.AreEqual(null, codeToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 12, 30).Date, codeToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 6, 24).Date, codeToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByInexistentCodeTypAsync()
    {
        var codes = await controller.GetByCodeTypAsync(672941).ConfigureAwait(false);
        Assert.AreEqual(0, codes.Count());
    }
}
