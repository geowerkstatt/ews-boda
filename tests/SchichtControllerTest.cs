using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using static EWS.Helpers;

namespace EWS;

[TestClass]
public class SchichtControllerTest
{
    private EwsContext context;
    private SchichtController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new SchichtController(context) { ControllerContext = GetControllerContext() };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task AddInvalidSchichtReturnsBadRequest()
    {
        var newSchicht = new Schicht { Bemerkung = "Multiple red lobsters on the playground." };
        var response = await controller.CreateAsync(newSchicht).ConfigureAwait(false) as ObjectResult;

        Assert.IsInstanceOfType(response, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.AreEqual("An error occurred while saving the entity changes.", ((ProblemDetails)response.Value!).Detail);
    }

    [TestMethod]
    public async Task AddMinimalSchichtReturnsCreatedResult()
    {
        var newSchicht = new Schicht
        {
            BohrprofilId = 50614,
            CodeSchichtId = 20093,
            Tiefe = 63877,
            QualitaetId = 369,
            HQualitaet = 11,
        };
        var response = await controller.CreateAsync(newSchicht).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newSchicht.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task AddFullSchichtReturnsCreatedResult()
    {
        var newSchicht = new Schicht
        {
            BohrprofilId = 55345,
            CodeSchichtId = 20033,
            Tiefe = 68,
            QualitaetId = 124,
            HQualitaet = 11,
            Bemerkung = "Physalis Mamnoun reaching new hights.",
            QualitaetBemerkung = "It only worked on full moon nights.",
        };
        var response = await controller.CreateAsync(newSchicht).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newSchicht.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task DeleteSchichtReturnsOk()
    {
        context.Schichten.Add(new Schicht
        {
            BohrprofilId = 52234,
            CodeSchichtId = 20094,
            Tiefe = 7799345.79f,
            QualitaetId = 80,
            HQualitaet = 11,
        });
        context.SaveChanges();
        Assert.AreEqual(14001, context.Schichten.Count());

        var schichtToDelete = context.Schichten.Single(s => s.Tiefe == 7799345.79f);
        var response = await controller.DeleteAsync(schichtToDelete.Id).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(OkResult));
        Assert.AreEqual(14000, context.Schichten.Count());
    }

    [TestMethod]
    public async Task TryDeleteInexistentSchichtReturnsNotFound()
    {
        var controller = new SchichtController(context) { ControllerContext = GetControllerContext(UserRole.Administrator) };
        var response = await controller.DeleteAsync(1600433).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        Assert.AreEqual(14000, context.Schichten.Count());
    }

    [TestMethod]
    public async Task EditSchichtReturnsOk()
    {
        var schichtToEdit = context.Schichten.Single(s => s.Id == 70013);
        var editedBemerkung = "We love peaches more than public spaces.";
        schichtToEdit.Bemerkung = editedBemerkung;
        var response = await controller.EditAsync(schichtToEdit).ConfigureAwait(false);
        Assert.AreEqual(context.Schichten.Single(s => s.Id == 70013).Bemerkung, editedBemerkung);
        Assert.IsInstanceOfType(response, typeof(OkResult));
    }

    [TestMethod]
    public async Task TryEditInexistentSchichtReturnsNotFound()
    {
        var inexistentSchicht = new Schicht
        {
            Id = 737445,
        };
        var response = await controller.EditAsync(inexistentSchicht).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
    }
}
