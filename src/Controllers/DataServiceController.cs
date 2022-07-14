using Microsoft.AspNetCore.Mvc;
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

    public DataServiceController(HttpClient client, ILogger<DataServiceController> logger)
    {
        this.client = client;
        this.logger = logger;

        client.BaseAddress = new Uri("https://geo.so.ch/api/data/v1/");
    }

    /// <summary>
    /// Retrieves the Gemeinde and Grundbuchnummern for the given points from the Data Service API.
    /// </summary>
    /// <param name="points">A list of <see cref="Point"/> to get the information for.</param>
    /// <returns>A <see cref="DataServiceResponse"/> containing the Gemeinde and Grundbuchnummern information.</returns>
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
                return Problem($"Found multiple gemeinden for input points <{points.Select(x => x.AsText())}>.");

            return new DataServiceResponse(gemeinden.SingleOrDefault(string.Empty), string.Join(",", grundbuchNummern));
        }
        catch (Exception)
        {
            return Problem("Error retrieving information from Data Service API.");
        }
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
