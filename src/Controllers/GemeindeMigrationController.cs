using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class GemeindeMigrationController : ControllerBase
{
    private readonly EwsContext context;
    private readonly DataService dataService;

    public GemeindeMigrationController(EwsContext context, DataService dataService)
    {
        this.context = context;
        this.dataService = dataService;
    }

    [HttpGet("/migrategemeinden")]
    public async Task<IActionResult> MigrateGemeinden([FromQuery] bool dryRun = true)
    {
        int found = 0;
        Dictionary<string, string?> notFound = new(), errors = new();

        // Only consider Standorte with Bohrungen
        var standorteToMigrate = context.Standorte.Include(x => x.Bohrungen).Where(x => x.Bohrungen != null && x.Bohrungen.Any()).ToList();

        await Parallel.ForEachAsync(standorteToMigrate, async (standort, _) =>
        {
            var standortId = $"{standort.Id}: {standort.Bezeichnung}";

            // Query data service for Gemeinde data with Bohrungen points
            var geometries = standort.Bohrungen!.Where(x => x.Geometrie != null).Select(x => x.Geometrie).ToList();

            try
            {
                var dataServiceResponse = await dataService.GetAsync(geometries!).ConfigureAwait(false);
                var gemeinde = dataServiceResponse.Gemeinde;
                if (string.IsNullOrEmpty(gemeinde))
                {
                    notFound.Add(standortId, string.Join(", ", geometries.Select(x => x.AsText())));
                }
                else
                {
                    found++;
                    standort.Gemeinde = gemeinde;
                }
            }
            catch (Exception ex)
            {
                errors.Add(standortId, ex.Message);
            }
        }).ConfigureAwait(false);

        if (!dryRun)
        {
            context.SaveChangesWithoutUpdatingChangeInformation();
        }

        return new JsonResult(new { Total = standorteToMigrate.Count, Success = found, NotFoundCount = notFound.Count, NotFound = notFound, ErrorCount = errors.Count, Errors = errors });
    }
}
