using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class StandortControllerTest
{
    private const string TestStandortBezeichnung = "Blue Jeans Pelati";
    private EwsContext context;
    private StandortController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new StandortController(context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task GetAllAsync()
    {
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
        Assert.AreEqual(new DateTime(2021, 8, 6).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortGrundbuchnummer()
    {
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
        Assert.AreEqual(new DateTime(2021, 10, 1).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortGrundbuchnummerSubstring()
    {
        var standorte = await controller.GetAsync(null, "kucmk1bwzaqt7c72").ConfigureAwait(false);

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
        Assert.AreEqual(new DateTime(2021, 10, 1).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnungSubstring()
    {
        var standorte = await controller.GetAsync(null, null, "Incredible").ConfigureAwait(false);

        Assert.AreEqual(351, standorte.Count());
        var standortToTest = standorte.Where(s => s.GrundbuchNr == "vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu").Single();

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
        Assert.AreEqual(new DateTime(2021, 10, 1).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnungSubstringIgnoringCase()
    {
        var standorte = await controller.GetAsync(null, null, "fiSH").ConfigureAwait(false);

        Assert.AreEqual(248, standorte.Count());
        var standortToTest = standorte.Where(s => s.GrundbuchNr == "vkflnsvlswy1nfbg4kucmk1bwzaqt7c72mba55vu").Single();

        Assert.AreEqual("Elijah Schmeler", standortToTest.AfuUser);
        Assert.AreEqual("Ghana", standortToTest.Bemerkung);
        Assert.AreEqual("Incredible Frozen Fish", standortToTest.Bezeichnung);
        Assert.AreEqual(false, standortToTest.FreigabeAfu);
        Assert.AreEqual(2568, standortToTest.Gemeinde);
        Assert.AreEqual(2, standortToTest.Bohrungen.Count);
        Assert.AreEqual("Elijah_Schmeler31", standortToTest.UserErstellung);
        Assert.AreEqual("Josefa_Effertz", standortToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 2, 12).Date, standortToTest.AfuDatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 10, 1).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByStandortBezeichnung()
    {
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
        Assert.AreEqual(new DateTime(2021, 3, 16).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 4, 18).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByErstellungsdatum()
    {
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
        Assert.AreEqual(new DateTime(2021, 11, 15, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 7, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetByMutationsdatum()
    {
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
        Assert.AreEqual(new DateTime(2021, 11, 3, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 5, 0, 0, 0, DateTimeKind.Utc).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetBySeveralParameters()
    {
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
        Assert.AreEqual(new DateTime(2021, 8, 1).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 1, 12).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task GetWithEmptyStrings()
    {
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
        Assert.AreEqual(new DateTime(2021, 8, 6).Date, standortToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 12, 9).Date, standortToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task AddInvalidStandortReturnsBadRequest()
    {
        var newStandort = new Standort { Bemerkung = "Various green toads blocking the road." };
        var response = await controller.CreateAsync(newStandort).ConfigureAwait(false) as ObjectResult;

        Assert.IsInstanceOfType(response, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.AreEqual("An error occurred while saving the entity changes.", ((ProblemDetails)response.Value!).Detail);
    }

    [TestMethod]
    public async Task AddMinimalStandortReturnsCreatedResult()
    {
        var newStandort = new Standort
        {
            Bezeichnung = TestStandortBezeichnung,
            UserErstellung = "Marky Mark Tribute Band Member",
        };
        var response = await controller.CreateAsync(newStandort).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newStandort.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task AddFullStandortReturnsCreatedResult()
    {
        var newStandort = new Standort
        {
            Bohrungen = new List<Bohrung>(),
            Bemerkung = "Mamady Doumbo",
            Gemeinde = 4321,
            GrundbuchNr = "hiKbSwsDBTXDyRf",
            Bezeichnung = TestStandortBezeichnung,
        };
        var response = await controller.CreateAsync(newStandort).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));
        await controller.DeleteAsync(newStandort.Id).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task DeleteStandortReturnsOk()
    {
        context.Standorte.Add(new Standort
        {
            Bemerkung = "Ismaila Kida",
            Gemeinde = 1234,
            GrundbuchNr = "cKt6QheQdD7WDjJ",
            Bezeichnung = TestStandortBezeichnung,
        });
        context.SaveChanges();
        Assert.AreEqual(6001, context.Standorte.Count());

        var standortToDelete = context.Standorte.Single(s => s.Bezeichnung == TestStandortBezeichnung);
        var response = await controller.DeleteAsync(standortToDelete.Id).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(OkResult));
        Assert.AreEqual(6000, context.Standorte.Count());
    }

    [TestMethod]
    public async Task TryDeleteInexistentStandortReturnsNotFound()
    {
        var controller = new StandortController(context);
        var response = await controller.DeleteAsync(1600433).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        Assert.AreEqual(6000, context.Standorte.Count());
    }

    [TestMethod]
    public async Task EditStandortReturnsOk()
    {
        var standortToEdit = context.Standorte.Single(s => s.Id == 31098);
        var editedBezeichnung = "We love Pasta more than Pesto";
        standortToEdit.Bezeichnung = editedBezeichnung;
        var response = await controller.EditAsync(standortToEdit).ConfigureAwait(false);
        Assert.AreEqual(context.Standorte.Single(s => s.Id == 31098).Bezeichnung, "We love Pasta more than Pesto");
        Assert.IsInstanceOfType(response, typeof(OkResult));
        Assert.AreEqual(6000, context.Standorte.Count());
    }

    [TestMethod]
    public async Task SubmitInvalidEditReturnsBadRequest()
    {
        var standortToEdit = context.Standorte.Single(s => s.Id == 31099);
        standortToEdit.Bezeichnung = null;

        var response = await controller.EditAsync(standortToEdit).ConfigureAwait(false) as ObjectResult;

        Assert.IsInstanceOfType(response, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.AreEqual("An error occurred while saving the entity changes.", ((ProblemDetails)response.Value!).Detail);
    }

    [TestMethod]
    public async Task TryEditInexistentStandortReturnsNotFound()
    {
        var inexistentStandort = new Standort
        {
            Id = 447375,
        };
        var response = await controller.EditAsync(inexistentStandort).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
    }
}
