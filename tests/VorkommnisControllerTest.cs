using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using static EWS.Helpers;

namespace EWS;

[TestClass]
public class VorkommnisControllerTest
{
    private EwsContext context;
    private VorkommnisController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new VorkommnisController(context) { ControllerContext = GetControllerContext() };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task AddInvalidVorkommnisReturnsBadRequest()
    {
        var newVorkommnis = new Vorkommnis { Bemerkung = "Many yellow oysters on the shovel." };
        var response = await controller.CreateAsync(newVorkommnis).ConfigureAwait(false) as ObjectResult;

        Assert.IsInstanceOfType(response, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.AreEqual("An error occurred while saving the entity changes.", ((ProblemDetails)response.Value!).Detail);
    }

    [TestMethod]
    public async Task AddMinimalVorkommnisReturnsCreatedResult()
    {
        var newVorkommnis = new Vorkommnis
        {
            BohrprofilId = 50614,
            TypId = 282,
            HQualitaet = 3,
            HTyp = 2,
        };
        var response = await controller.CreateAsync(newVorkommnis).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newVorkommnis.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task AddFullVorkommnisReturnsCreatedResult()
    {
        var newVorkommnis = new Vorkommnis
        {
            BohrprofilId = 55345,
            TypId = 328,
            Tiefe = 6341,
            QualitaetId = 330,
            HQualitaet = 3,
            HTyp =2,
            Bemerkung = "Physalis Mamnoun reaching new hights.",
            QualitaetBemerkung = "It only worked on full moon nights.",
        };
        var response = await controller.CreateAsync(newVorkommnis).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newVorkommnis.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task DeleteVorkommnisReturnsOk()
    {
        context.Vorkommnisse.Add(new Vorkommnis
        {
            BohrprofilId = 54872,
            Tiefe = 4699345.79f,
            TypId = 349,
            HQualitaet = 3,
            HTyp = 2,
        });
        context.SaveChanges();
        Assert.AreEqual(14001, context.Vorkommnisse.Count());

        var vorkommnisToDelete = context.Vorkommnisse.Single(s => s.Tiefe == 4699345.79f);
        var response = await controller.DeleteAsync(vorkommnisToDelete.Id).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(OkResult));
        Assert.AreEqual(14000, context.Vorkommnisse.Count());
    }

    [TestMethod]
    public async Task TryDeleteInexistentVorkommnisReturnsNotFound()
    {
        var controller = new VorkommnisController(context) { ControllerContext = GetControllerContext(UserRole.Administrator) };
        var response = await controller.DeleteAsync(97736145).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        Assert.AreEqual(14000, context.Vorkommnisse.Count());
    }

    [TestMethod]
    public async Task EditVorkommnisReturnsOk()
    {
        var vorkommnisToEdit = context.Vorkommnisse.Single(s => s.Id == 90096);
        var editedBemerkung = "We love peaches more than public spaces.";
        vorkommnisToEdit.Bemerkung = editedBemerkung;
        var response = await controller.EditAsync(vorkommnisToEdit).ConfigureAwait(false);
        Assert.AreEqual(context.Vorkommnisse.Single(s => s.Id == 90096).Bemerkung, editedBemerkung);
        Assert.IsInstanceOfType(response, typeof(OkResult));
    }

    [TestMethod]
    public async Task TryEditInexistentVorkommnisReturnsNotFound()
    {
        var inexistentVorkommnis = new Vorkommnis
        {
            Id = 97736145,
        };
        var response = await controller.EditAsync(inexistentVorkommnis).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
    }
}
