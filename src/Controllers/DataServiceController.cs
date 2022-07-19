using EWS.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Text.Json;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class DataServiceController : ControllerBase
{
    /// <summary>The well-known layer names for querying data.</summary>
    private const string GemeindeLayer = "ch.so.agi.gemeindegrenzen.data";
    private const string GrundstueckLayer = "ch.so.agi.av.grundstuecke.rechtskraeftig.data";

    private readonly HttpClient client;
    private readonly ILogger<DataServiceController> logger;
    private readonly EwsContext context;

    public DataServiceController(HttpClient client, ILogger<DataServiceController> logger, EwsContext context)
    {
        this.client = client;
        this.logger = logger;
        this.context = context;

        client.BaseAddress = new Uri("https://geo.so.ch/api/data/v1/");
    }

    /// <summary>
    /// Retrieves the Gemeinde and Grundbuchnummern for the given points from the Data Service API.
    /// </summary>
    /// <param name="points">A list of <see cref="Point"/> to get the information for.</param>
    /// <returns>A <see cref="DataServiceResponse"/> containing the Gemeinde and Grundbuchnummern information.</returns>
    [Authorize(Policy = PolicyNames.Extern)]
    [HttpGet]
    public async Task<ActionResult<DataServiceResponse>> GetAsync([FromQuery] List<Point> points)
    {
        try
        {
            List<string?> gemeinden = new(), grundbuchNummern = new();
            foreach (var point in points)
            {
                var boundingBox = $"{point.X},{point.Y},{point.X},{point.Y}";

                var gemeinde = await GetDataServiceApiResponse($"{GemeindeLayer}/?bbox={boundingBox}", "gemeindename").ConfigureAwait(false);
                if (!gemeinden.Contains(gemeinde))
                    gemeinden.Add(gemeinde);

                var grundbuchnummer = await GetDataServiceApiResponse($"{GrundstueckLayer}/?bbox={boundingBox}", "nummer").ConfigureAwait(false);
                if (!grundbuchNummern.Contains(grundbuchnummer))
                    grundbuchNummern.Add(grundbuchnummer);
            }

            if (gemeinden.Count > 1)
                return Problem($"Found multiple gemeinden for input points <{string.Join(", ", points.Select(x => x.AsText()))}>.");

            return new DataServiceResponse(gemeinden.SingleOrDefault(string.Empty), string.Join(",", grundbuchNummern));
        }
        catch (Exception)
        {
            return Problem("Error retrieving information from Data Service API.");
        }
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
            var dataServiceResponse = await GetAsync(geometries!).ConfigureAwait(false);
            if (dataServiceResponse.Value != null)
            {
                var gemeinde = dataServiceResponse.Value.Gemeinde;
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
            else
            {
                var error = (ObjectResult)dataServiceResponse.Result!;
                errors.Add(standortId, ((ProblemDetails)error.Value!).Detail);
            }
        }).ConfigureAwait(false);

        if (!dryRun)
        {
            context.SaveChangesWithoutUpdatingChangeInformation();
        }

        return new JsonResult(new { Total = standorteToMigrate.Count, Success = found, NotFoundCount = notFound.Count, NotFound = notFound, ErrorCount = errors.Count, Errors = errors });
    }

    private async Task<string?> GetDataServiceApiResponse(string requestUrl, string propertyName)
    {
        try
        {
            using var response = await client.GetAsync(new Uri(requestUrl, UriKind.Relative)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadFromJsonAsync<JsonElement>().ConfigureAwait(false);
            if (jsonContent.TryGetProperty("features", out var features) && features.EnumerateArray().Any())
            {
                return features[0].GetProperty("properties").GetProperty(propertyName).GetString();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving information from Data Service API. Request url was: {RequestUrl}.", requestUrl);
            throw;
        }
    }
}
