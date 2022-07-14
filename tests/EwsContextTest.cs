using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class EwsContextTest
{
    [TestMethod]
    public async Task CascadeDelete()
    {
        // Create new tree (Standort -> Bohrungen -> Bohrprofile) by adding one by one.
        var newStandort = new Standort
        {
            Bezeichnung = "VIOLETSOURCE",
            Bemerkung = "Lorem ipsum dolor sit amet.",
            Gemeinde = "QUADRANT",
            GrundbuchNr = "c99abf9d-71fb-4310-9c84-412786744a52",
        };
        await new StandortController(ContextFactory.CreateContext()).CreateAsync(newStandort).ConfigureAwait(false);

        var newBohrung = new Bohrung
        {
            Bezeichnung = "BLUEWAFFLE",
            StandortId = newStandort.Id,
            HQualitaetId = 3,
            HAblenkung = 9,
        };
        await new BohrungController(ContextFactory.CreateContext()).CreateAsync(newBohrung).ConfigureAwait(false);

        var newBohrprofil = new Bohrprofil
        {
            Bemerkung = "Morbi ut lectus ligula.",
            BohrungId = newBohrung.Id,
            HQualitaetId = 12,
            HTektonikId = 10,
            HFormationEndtiefeId = 5,
            HFormationFelsId = 5,
        };
        await new BohrprofilController(ContextFactory.CreateContext()).CreateAsync(newBohrprofil).ConfigureAwait(false);

        // Assert tree
        Assert.IsNotNull(await ContextFactory.CreateContext().Standorte.FindAsync(newStandort.Id).ConfigureAwait(false));
        Assert.IsNotNull(await ContextFactory.CreateContext().Bohrungen.FindAsync(newBohrung.Id).ConfigureAwait(false));
        Assert.IsNotNull(await ContextFactory.CreateContext().Bohrprofile.FindAsync(newBohrprofil.Id).ConfigureAwait(false));

        var standort = ContextFactory.CreateContext().Standorte
            .Include(x => x.Bohrungen)
            .ThenInclude(x => x.Bohrprofile)
            .Where(x => x.Id == newStandort.Id)
            .Single();
        Assert.AreEqual("VIOLETSOURCE", standort.Bezeichnung);
        Assert.AreEqual("BLUEWAFFLE", standort.Bohrungen.Single().Bezeichnung);
        Assert.AreEqual("Morbi ut lectus ligula.", standort.Bohrungen.Single().Bohrprofile.Single().Bemerkung);

        // Update Standort with empty Bohrungen collection should not delete Bohrungen from the specified Standort
        var updatedNewStandort = ContextFactory.CreateContext().Standorte.AsNoTracking().Where(x => x.Id == newStandort.Id).First();
        updatedNewStandort.Bezeichnung = "VIOLETYARD REV4";
        updatedNewStandort.Erstellungsdatum = DateTime.Now.ToUniversalTime();
        Assert.IsNull(updatedNewStandort.Bohrungen);
        await new StandortController(ContextFactory.CreateContext()).EditAsync(updatedNewStandort).ConfigureAwait(false);

        // Assert updated Standort Bezeichnung and Bohrungen
        Assert.IsNotNull(await ContextFactory.CreateContext().Standorte.FindAsync(newStandort.Id).ConfigureAwait(false));
        Assert.IsNotNull(await ContextFactory.CreateContext().Bohrungen.FindAsync(newBohrung.Id).ConfigureAwait(false));
        Assert.IsNotNull(await ContextFactory.CreateContext().Bohrprofile.FindAsync(newBohrprofil.Id).ConfigureAwait(false));

        var updatedStandort = ContextFactory.CreateContext().Standorte
            .Include(x => x.Bohrungen)
            .ThenInclude(x => x.Bohrprofile)
            .Where(x => x.Id == newStandort.Id)
            .Single();
        Assert.AreEqual("VIOLETYARD REV4", updatedStandort.Bezeichnung);
        Assert.AreEqual("BLUEWAFFLE", updatedStandort.Bohrungen.Single().Bezeichnung);
        Assert.AreEqual("Morbi ut lectus ligula.", updatedStandort.Bohrungen.Single().Bohrprofile.Single().Bemerkung);

        // Delete entire tree (Standort -> Bohrungen -> Bohrprofile)
        await new StandortController(ContextFactory.CreateContext()).DeleteAsync(newStandort.Id).ConfigureAwait(false);
        Assert.IsNull(await ContextFactory.CreateContext().Standorte.FindAsync(newStandort.Id).ConfigureAwait(false));
        Assert.IsNull(await ContextFactory.CreateContext().Bohrungen.FindAsync(newBohrung.Id).ConfigureAwait(false));
        Assert.IsNull(await ContextFactory.CreateContext().Bohrprofile.FindAsync(newBohrprofil.Id).ConfigureAwait(false));
    }

    [TestMethod]
    public async Task UpdateFreigabeAfuFieldsOnFreigabe()
    {
        var standortController = new StandortController(ContextFactory.CreateContext());
        var standort = new Standort
        {
            Bezeichnung = "ENTOURAGEWHISPER",
            Bemerkung = "Lorem ipsum dolor sit amet.",
            Gemeinde = "Herbetswil",
            GrundbuchNr = "9fd82ef7-e8ff-4e95-80fb-24f9c3eaef91",
        };
        await standortController.CreateAsync(standort).ConfigureAwait(false);

        // Assert Standort (not yet released)
        Assert.AreEqual("ENTOURAGEWHISPER", standort.Bezeichnung);
        Assert.AreEqual(false, standort.FreigabeAfu);
        Assert.IsNull(standort.AfuUser);
        Assert.IsNull(standort.AfuDatum);

        standort.FreigabeAfu = true;
        await standortController.EditAsync(standort).ConfigureAwait(false);

        // Assert Standort (released)
        Assert.AreEqual("ENTOURAGEWHISPER", standort.Bezeichnung);
        Assert.AreEqual(true, standort.FreigabeAfu);
        Assert.IsNotNull(standort.AfuUser);
        Assert.IsNotNull(standort.AfuDatum);

        // Update Standort again and assert original Freigabe fields don't change.
        var originalAfuUser = standort.AfuUser;
        var origianlAfuDatum = standort.AfuDatum.Value.Ticks;

        standort.Bezeichnung = "VIOLENTFELONY";
        await standortController.EditAsync(standort).ConfigureAwait(false);

        Assert.AreEqual("VIOLENTFELONY", standort.Bezeichnung);
        Assert.AreEqual(true, standort.FreigabeAfu);
        Assert.AreEqual(originalAfuUser, standort.AfuUser);
        Assert.AreEqual(origianlAfuDatum, standort.AfuDatum.Value.Ticks);

        // Remove Freigabe and assert fields get restored to their default values.
        standort.FreigabeAfu = false;
        await standortController.EditAsync(standort).ConfigureAwait(false);

        Assert.AreEqual("VIOLENTFELONY", standort.Bezeichnung);
        Assert.AreEqual(false, standort.FreigabeAfu);
        Assert.IsNull(standort.AfuUser);
        Assert.IsNull(standort.AfuDatum);

        // Cleanup
        await standortController.DeleteAsync(standort.Id).ConfigureAwait(false);
    }
}
