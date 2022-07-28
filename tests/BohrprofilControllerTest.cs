using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class BohrprofilControllerTest
{
    private HttpClient httpClient;
    private EwsContext context;
    private BohrprofilController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        httpClient = new HttpClient();
        context = ContextFactory.CreateContext();
        controller = new BohrprofilController(ContextFactory.CreateContext());
    }

    [TestCleanup]
    public void TestCleanup()
    {
        httpClient.Dispose();
        context.Dispose();
    }

    [TestMethod]
    public async Task GetByIdAsync()
    {
        var bohrprofilId = 50023;
        var actionResult = await controller.GetByIdAsync(bohrprofilId).ConfigureAwait(false);
        var bohrprofilToTest = actionResult.Value;
        Assert.AreEqual(40062, bohrprofilToTest.BohrungId);
        Assert.AreEqual("Bedfordshire", bohrprofilToTest.Bemerkung);
        Assert.AreEqual(18416, bohrprofilToTest.Kote);
        Assert.AreEqual(0, bohrprofilToTest.Schichten.Count);
        Assert.AreEqual(1, bohrprofilToTest.Vorkommnisse.Count);
        Assert.AreEqual(new DateTime(2021, 10, 5).Date, bohrprofilToTest.Datum!.Value.Date);
        Assert.AreEqual(16466, bohrprofilToTest.Endteufe);
        Assert.AreEqual(346, bohrprofilToTest.TektonikId);
        Assert.AreEqual(313, bohrprofilToTest.FormationEndtiefeId);
        Assert.AreEqual(133, bohrprofilToTest.FormationFelsId);
        Assert.AreEqual(15, bohrprofilToTest.QualitaetId);
        Assert.AreEqual(12, bohrprofilToTest.HQualitaet);
        Assert.AreEqual(5, bohrprofilToTest.HFormationEndtiefe);
        Assert.AreEqual(5, bohrprofilToTest.HFormationFels);
        Assert.AreEqual(10, bohrprofilToTest.HTektonik);
        Assert.AreEqual(null, bohrprofilToTest.QualitaetBemerkung);
        Assert.AreEqual("Pablo.Kessler", bohrprofilToTest.UserErstellung);
        Assert.AreEqual(null, bohrprofilToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 11, 8).Date, bohrprofilToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 1, 2).Date, bohrprofilToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByIdWithInexistentBohrprofil()
    {
        var inexistentBohrprofilId = 56132584;
        var actionResult = await controller.GetByIdAsync(inexistentBohrprofilId).ConfigureAwait(false);
        Assert.AreEqual(typeof(NotFoundResult), actionResult.Result.GetType());
    }
}
