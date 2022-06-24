using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class StandortController : ControllerBase
{
    private readonly EwsContext context;
    public StandortController(EwsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Standort>> GetAsync(
         int? gemeindenummer = null, string? gbnummer = null, string? bezeichnung = null, DateTime? erstellungsdatum = null, DateTime? mutationsdatum = null)
    {
        var standorte = context.Standorte.Include(s => s.Bohrungen).AsQueryable();
        if (gemeindenummer != null)
        {
            standorte = standorte.Where(s => s.Gemeinde == gemeindenummer);
        }

        if (!string.IsNullOrEmpty(gbnummer))
        {
            standorte = standorte.Where(s => s.GrundbuchNr == gbnummer);
        }

        if (!string.IsNullOrEmpty(bezeichnung))
        {
            standorte = standorte.Where(s => s.Bezeichnung == bezeichnung);
        }

        // Use universal time zone to convert time.
        TimeZoneInfo ut = TimeZoneInfo.Utc;

        if (erstellungsdatum != null)
        {
            standorte = standorte.Where(s => s.Erstellungsdatum.Date == TimeZoneInfo.ConvertTimeToUtc(erstellungsdatum.Value, ut).Date);
        }

        if (mutationsdatum != null)
        {
            standorte = standorte.Where(s => s.Mutationsdatum != null && s.Mutationsdatum!.Value.Date == TimeZoneInfo.ConvertTimeToUtc(mutationsdatum.Value, ut).Date);
        }

        return await standorte.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
