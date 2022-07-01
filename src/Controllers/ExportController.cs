using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class ExportController : ControllerBase
{
    private readonly EwsContext context;

    public ExportController(EwsContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously exports the bohrung.data_export database view into a comma-separated file format (CSV).
    /// The first record represents the header containing the specified list of field names.
    /// The output is created by the PostgreSQL COPY command.
    /// </summary>
    [HttpGet]
    public async Task<ContentResult> GetAsync(CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(context.Database.GetDbConnection().ConnectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        using var reader = await connection.BeginTextExportAsync(
            "COPY (SELECT * FROM bohrung.data_export) TO STDOUT WITH DELIMITER ',' CSV HEADER;",
            cancellationToken).ConfigureAwait(false);

        return Content(await reader.ReadToEndAsync().ConfigureAwait(false), "text/csv", Encoding.UTF8);
    }
}
