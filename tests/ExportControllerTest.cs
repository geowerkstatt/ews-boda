using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class ExportControllerTest
{
    private EwsContext context;

    [TestInitialize]
    public void TestInitialize() => context = ContextFactory.CreateContext();

    [TestCleanup]
    public void TestCleanup() => context.Dispose();

    [TestMethod]
    public async Task GetAsync()
    {
        var httpContext = new DefaultHttpContext();
        var controller = new ExportController(CreateConfiguration());
        controller.ControllerContext.HttpContext = httpContext;
        var response = await controller.GetAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(ContentResult));
        Assert.AreEqual("text/csv; charset=utf-8", response.ContentType);
        Assert.AreEqual("attachment; filename=data_export.csv", httpContext.Response.Headers["Content-Disposition"].ToString());

        var expectedHeader = "standort.standort_id,standort.bezeichnung,standort.bemerkung,standort.anzbohrloch,standort.gbnummer,standort.freigabe_afu,standort.afu_usr,standort.afu_date,bohrung.bohrung_id,bohrung.bezeichnung,bohrung.bemerkung,bohrung.datum,bohrung.durchmesserbohrloch,bohrung.ablenkung,bohrung.quali,bohrung.quali_text,bohrung.quali_kurztext,bohrung.qualibem,bohrung.quelleref,bohrung.h_quali,bohrung.h_quali_text,bohrung.h_quali_kurztext,bohrung.X,bohrung.Y,bohrprofil.bohrprofil_id,bohrprofil.datum,bohrprofil.bemerkung,bohrprofil.kote,bohrprofil.endtiefe,bohrprofil.tektonik,bohrprofil.tektonik_text,bohrprofil.tektonik_kurztext,bohrprofil.fmfelso,bohrprofil.fmfelso_text,bohrprofil.fmfelso_kurztext,bohrprofil.fmeto,bohrprofil.fmeto_text,bohrprofil.fmeto_kurztext,bohrprofil.quali,bohrprofil.quali_text,bohrprofil.quali_kurztext,bohrprofil.qualibem,bohrprofil.h_quali,bohrprofil.h_quali_text,bohrprofil.h_quali_kurztext,bohrprofil.h_tektonik,bohrprofil.h_tektonik_text,bohrprofil.h_tektonik_kurztext,bohrprofil.h_fmeto,bohrprofil.h_fmeto_text,bohrprofil.h_fmeto_kurztext,bohrprofil.h_fmfelso,bohrprofil.h_fmfelso_text,bohrprofil.h_fmfelso_kurztext,schicht.schicht_id,schicht.tiefe,schicht.quali,schicht.quali_text,schicht.quali_kurztext,schicht.qualibem,schicht.bemerkung,schicht.h_quali,schicht.h_quali_text,schicht.h_quali_kurztext,codeschicht.kurztext,codeschicht.text,codeschicht.sort,vorkommnis.vorkommnis_id,vorkommnis.typ,vorkommnis.typ_text,vorkommnis.typ_kurztext,vorkommnis.tiefe,vorkommnis.bemerkung,vorkommnis.quali,vorkommnis.quali_text,vorkommnis.quali_kurztext,vorkommnis.qualibem,vorkommnis.h_quali,vorkommnis.h_quali_text,vorkommnis.h_quali_kurztext,vorkommnis.h_typ,vorkommnis.h_typ_text,vorkommnis.h_typ_kurztext";

        Assert.AreEqual(expectedHeader, response.Content.Split('\n')[0]);
        Assert.AreEqual(31191, response.Content.Split('\n').Length);
    }

    [TestMethod]
    public async Task GetExportDataShouldEliminateNewlineCharacters()
    {
        var testBemerkung = "New\nline\rcharacters\n\rall\nover";
        var expectedFormattedText = "New line characters all over";

        var newStandort = new Standort
        {
            Bezeichnung = "Standort Test",
            Bemerkung = testBemerkung,
        };
        context.Standorte.Add(newStandort);
        await context.SaveChangesAsync();

        var newBohrung = new Bohrung
        {
            Bezeichnung = "Bohrung Test",
            StandortId = newStandort.Id,
            HAblenkung = 9,
            HQualitaet = 3,
            Bemerkung = testBemerkung,
        };

        context.Bohrungen.Add(newBohrung);
        await context.SaveChangesAsync();

        var newBohrprofil = new Bohrprofil
        {
            BohrungId = newBohrung.Id,
            HQualitaet = 12,
            HFormationEndtiefe = 5,
            HTektonik = 10,
            HFormationFels = 5,
            Bemerkung = testBemerkung,
        };
        context.Bohrprofile.Add(newBohrprofil);
        await context.SaveChangesAsync();

        context.Schichten.Add(new Schicht
        {
            BohrprofilId = newBohrprofil.Id,
            CodeSchichtId = 20094,
            Tiefe = 10.79f,
            HQualitaet = 11,
            Bemerkung = testBemerkung,
        });

        context.Vorkommnisse.Add(new Vorkommnis
        {
            BohrprofilId = newBohrprofil.Id,
            TypId = 349,
            HQualitaet = 3,
            HTyp = 2,
            Bemerkung = testBemerkung,
        });

        await context.SaveChangesAsync();
        var controller = new ExportController(CreateConfiguration());
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext = httpContext;
        var response = await controller.GetAsync(CancellationToken.None).ConfigureAwait(false);
        var csvContent = response.Content;

        Assert.IsFalse(csvContent.Contains(testBemerkung),
            "CSV should not contain the original text with newlines");

        Assert.IsTrue(csvContent.Contains(expectedFormattedText),
            "CSV should contain the text with newlines replaced by spaces");

        // Delete all created entities
        context.Vorkommnisse.RemoveRange(context.Vorkommnisse.Where(v => v.BohrprofilId == newBohrprofil.Id));
        context.Schichten.RemoveRange(context.Schichten.Where(s => s.BohrprofilId == newBohrprofil.Id));
        context.Bohrprofile.Remove(newBohrprofil);
        context.Bohrungen.Remove(newBohrung);
        context.Standorte.Remove(newStandort);
        await context.SaveChangesAsync();
    }

    private IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ConnectionStrings:BohrungContext", ContextFactory.ConnectionString },
        }).Build();
}
