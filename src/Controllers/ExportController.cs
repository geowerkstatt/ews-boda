using EWS.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class ExportController : ControllerBase
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public ExportController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// Asynchronously exports the bohrung.data_export database view into a comma-separated file format (CSV).
    /// The first record represents the header containing the specified list of field names.
    /// The output is created by the PostgreSQL COPY command.
    /// </summary>
    [HttpGet]
    public async Task<ContentResult> GetAsync(CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(configuration.GetConnectionString("BohrungContext"));
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        using var reader = await connection.BeginTextExportAsync(
            "COPY (SELECT * FROM bohrung.data_export) TO STDOUT WITH DELIMITER ',' CSV HEADER;",
            cancellationToken).ConfigureAwait(false);

        Response.Headers.ContentDisposition = "attachment; filename=data_export.csv";
        return Content(await reader.ReadToEndAsync().ConfigureAwait(false), "text/csv", Encoding.UTF8);
    }
}
