using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using static EWS.Helpers;

namespace EWS;

[TestClass]
public class BohrungControllerTest
{
    private EwsContext context;
    private BohrungController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new BohrungController(context) { ControllerContext = GetControllerContext() };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task GetByIdAsync()
    {
        var bohrungId = 40097;
        var actionResult = await controller.GetByIdAsync(bohrungId).ConfigureAwait(false);
        var okResult = actionResult as OkObjectResult;
        var bohrungToTest = okResult.Value as Bohrung;
        Assert.AreEqual(32, bohrungToTest.AblenkungId);
        Assert.AreEqual("Exclusive transitional migration", bohrungToTest.Bemerkung);
        Assert.AreEqual("Generic Steel Chips", bohrungToTest.Bezeichnung);
        Assert.AreEqual(1, bohrungToTest.Bohrprofile.Count);
        Assert.AreEqual(new DateTime(2021, 8, 2).Date, bohrungToTest.Datum!.Value.Date);
        Assert.AreEqual(709, bohrungToTest.DurchmesserBohrloch);
        Assert.AreEqual("POINT (2606352 1243296)", bohrungToTest.Geometrie.ToString());
        Assert.AreEqual(9, bohrungToTest.HAblenkung);
        Assert.AreEqual(3, bohrungToTest.HQualitaet);
        Assert.AreEqual(null, bohrungToTest.Qualitaet);
        Assert.AreEqual("I saw one of these in Spratly Islands and I bought one.", bohrungToTest.QualitaetBemerkung);
        Assert.AreEqual(397, bohrungToTest.QualitaetId);
        Assert.AreEqual("Fisher - Nicolas", bohrungToTest.QuelleRef);
        Assert.AreEqual(34419, bohrungToTest.StandortId);
        Assert.AreEqual("Colin10", bohrungToTest.UserErstellung);
        Assert.AreEqual("Audrey96", bohrungToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 3, 24).Date, bohrungToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 8, 23).Date, bohrungToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByIdWithInexistentStandort()
    {
        var inexistentBohrungId = 61325584;
        var actionResult = await controller.GetByIdAsync(inexistentBohrungId).ConfigureAwait(false);
        Assert.AreEqual(typeof(NotFoundResult), actionResult.GetType());
    }
}
