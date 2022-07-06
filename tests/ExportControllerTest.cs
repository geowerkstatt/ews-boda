using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
        var controller = new ExportController(context);
        controller.ControllerContext.HttpContext = httpContext;
        var response = await controller.GetAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(ContentResult));
        Assert.AreEqual("text/csv; charset=utf-8", response.ContentType);
        Assert.AreEqual("attachment; filename=data_export.csv", httpContext.Response.Headers["Content-Disposition"].ToString());

        var expectedHeader = "standort.standort_id,standort.bezeichnung,standort.bemerkung,standort.anzbohrloch,standort.gbnummer,standort.freigabe_afu,standort.afu_usr,standort.afu_date,bohrung.bohrung_id,bohrung.bezeichnung,bohrung.bemerkung,bohrung.datum,bohrung.durchmesserbohrloch,bohrung.ablenkung,bohrung.quali,bohrung.qualibem,bohrung.quelleref,bohrung.h_quali,bohrung.X,bohrung.Y,bohrprofil.bohrprofil_id,bohrprofil.datum,bohrprofil.bemerkung,bohrprofil.kote,bohrprofil.endtiefe,bohrprofil.tektonik,bohrprofil.fmfelso,bohrprofil.fmeto,bohrprofil.quali,bohrprofil.qualibem,bohrprofil.h_quali,bohrprofil.h_tektonik,bohrprofil.h_fmeto,bohrprofil.h_fmfelso,schicht.schicht_id,schicht.tiefe,schicht.quali,schicht.qualibem,schicht.bemerkung,schicht.h_quali,codeschicht.kurztext,codeschicht.text,codeschicht.sort,vorkommnis.vorkommnis_id,vorkommnis.typ,vorkommnis.tiefe,vorkommnis.bemerkung,vorkommnis.quali,vorkommnis.qualibem,vorkommnis.h_quali,vorkommnis.h_typ";

        Assert.AreEqual(expectedHeader, response.Content.Split('\n')[0]);
        Assert.AreEqual(31191, response.Content.Split('\n').Length);
    }
}
