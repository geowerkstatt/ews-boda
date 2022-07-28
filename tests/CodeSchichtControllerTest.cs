using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class CodeSchichtControllerTest
{
    private EwsContext context;
    private CodeSchichtController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new CodeSchichtController(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task GetAsync()
    {

        // get codesschichten
        var codeSchichten = await controller.GetAsync().ConfigureAwait(false);
        Assert.AreEqual(100, codeSchichten.Count());

        var idToTest = 20095;
        var codeToTest = codeSchichten.Single(b => b.Id == idToTest);

        Assert.AreEqual("Agent parse SMTP", codeToTest.Kurztext);
        Assert.AreEqual(5901, codeToTest.Sortierung);
        Assert.AreEqual("http://sim.org/unbranded-metal-bacon/mozambique/primary", codeToTest.Text);
        Assert.AreEqual("Harry91", codeToTest.UserErstellung);
        Assert.AreEqual("Luz_Jaskolski26", codeToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 2, 24).Date, codeToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 2, 10).Date, codeToTest.Mutationsdatum!.Value.Date);
    }
}
