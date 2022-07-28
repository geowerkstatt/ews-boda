using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetTopologySuite.Geometries;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static EWS.Helpers;

namespace EWS;

[TestClass]
public class BohrungControllerTest
{
    private HttpClient httpClient;
    private EwsContext context;
    private BohrungController bohrungController;
    private StandortController standortController;
    private Standort standort;

    [TestInitialize]
    public async Task TestInitialize()
    {
        httpClient = new HttpClient();
        context = ContextFactory.CreateContext();
        bohrungController = new BohrungController(httpClient, new Mock<ILogger<DataServiceController>>().Object, ContextFactory.CreateContext());
        standortController = new StandortController(ContextFactory.CreateContext()) { ControllerContext = GetControllerContext() };

        // Setup standort for manipulating Bohrungen
        standort = new Standort { Bezeichnung = "THUNDER-V", UserErstellung = "BohrungControllerTest" };
        var result = await standortController.CreateAsync(standort);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        // Cleanup standort with associated bohrungen
        await standortController.DeleteAsync(standort.Id);
        Assert.IsInstanceOfType(await standortController.GetByIdAsync(standort.Id), typeof(NotFoundResult));

        httpClient.Dispose();
        await context.DisposeAsync();
    }

    [TestMethod]
    public async Task GetByIdAsync()
    {
        var bohrungId = 40097;
        var actionResult = await bohrungController.GetByIdAsync(bohrungId).ConfigureAwait(false);
        var bohrungToTest = actionResult.Value;
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
        var actionResult = await bohrungController.GetByIdAsync(inexistentBohrungId).ConfigureAwait(false);
        Assert.AreEqual(typeof(NotFoundResult), actionResult.Result.GetType());
    }

    [TestMethod]
    public async Task CreateEditAndDeleteAsyncWithoutGeometry()
    {
        // Add
        var bohrung = new Bohrung
        {
            Bezeichnung = "PERFECTBOOK",
            StandortId = standort.Id,
            HAblenkung = 9,
            HQualitaet = 3,
        };

        Assert.IsInstanceOfType(await bohrungController.CreateAsync(bohrung), typeof(CreatedAtActionResult));
        bohrung = await context.Bohrungen.FindAsync(bohrung.Id);
        Assert.AreEqual("PERFECTBOOK", bohrung.Bezeichnung);

        // Edit
        bohrung.Bezeichnung = "RESERVEOLIVE";
        Assert.IsInstanceOfType(await bohrungController.EditAsync(bohrung), typeof(OkResult));
        bohrung = await context.Bohrungen.FindAsync(bohrung.Id);
        Assert.AreEqual("RESERVEOLIVE", bohrung.Bezeichnung);

        // Delete
        await bohrungController.DeleteAsync(bohrung.Id);
        Assert.IsInstanceOfType((await bohrungController.GetByIdAsync(bohrung.Id)).Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task CreateAndEditAsyncWithGeometryShouldUpdateStandortInformation()
    {
        // Add
        var bohrung = new Bohrung
        {
            Bezeichnung = "LIONMAGIC",
            StandortId = standort.Id,
            Geometrie = new Point(2605532, 1229554), // Gemeinde Langendorf
            HAblenkung = 9,
            HQualitaet = 3,
        };

        Assert.IsInstanceOfType(await bohrungController.CreateAsync(bohrung), typeof(CreatedAtActionResult));
        bohrung = await context.Bohrungen.FindAsync(bohrung.Id);
        Assert.AreEqual("LIONMAGIC", bohrung.Bezeichnung);

        standort = await context.Standorte.AsNoTracking().SingleAsync(x => x.Id == standort.Id);
        Assert.AreEqual("Langendorf", standort.Gemeinde);
        Assert.AreEqual("1950", standort.GrundbuchNr);

        // Edit
        bohrung.Bezeichnung = "WINDHAIR";
        bohrung.Geometrie = new Point(2605164, 1228521); // Gemeinde Bellach

        Assert.IsInstanceOfType(await new BohrungController(new(), new Mock<ILogger<DataServiceController>>().Object, ContextFactory.CreateContext()).EditAsync(bohrung), typeof(OkResult));
        bohrung = await context.Bohrungen.FindAsync(bohrung.Id);
        Assert.AreEqual("WINDHAIR", bohrung.Bezeichnung);

        standort = await context.Standorte.AsNoTracking().SingleAsync(x => x.Id == standort.Id);
        Assert.AreEqual("Bellach", standort.Gemeinde);
        Assert.AreEqual("730", standort.GrundbuchNr);

        // Delete
        await new BohrungController(new(), new Mock<ILogger<DataServiceController>>().Object, ContextFactory.CreateContext()).DeleteAsync(bohrung.Id);
        Assert.IsInstanceOfType((await new BohrungController(new(), new Mock<ILogger<DataServiceController>>().Object, ContextFactory.CreateContext()).GetByIdAsync(bohrung.Id)).Result, typeof(NotFoundResult));

        standort = await context.Standorte.AsNoTracking().SingleAsync(x => x.Id == standort.Id);
        Assert.AreEqual("", standort.Gemeinde);
        Assert.AreEqual("", standort.GrundbuchNr);
    }

    [TestMethod]
    public async Task CreateAsyncWithGeometryOutsideOfSolothurnShouldReturnProblem()
    {
        var bohrung = new Bohrung
        {
            Bezeichnung = "FRUGALRECORD",
            StandortId = standort.Id,
            Geometrie = new Point(2759206, 1191408), // Chur
            HAblenkung = 9,
            HQualitaet = 3,
        };

        var result = await bohrungController.CreateAsync(bohrung) as ObjectResult;
        Assert.AreEqual("Call to Data Service API did not yield any results. The supplied geometry 'POINT (2759206 1191408)' may not lie in Kanton Solothurn.", ((ProblemDetails)result.Value!).Detail);
    }
}
