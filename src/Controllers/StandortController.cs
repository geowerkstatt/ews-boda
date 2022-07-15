using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class StandortController : EwsControllerBase<Standort>
{
    public StandortController(EwsContext context)
        : base(context)
    {
    }

    [HttpGet]
    public async Task<IEnumerable<Standort>> GetAsync(
         string? gemeinde = null, string? gbnummer = null, string? bezeichnung = null, DateTime? erstellungsdatum = null, DateTime? mutationsdatum = null)
    {
        var standorte = Context.Standorte.Include(s => s.Bohrungen).ThenInclude(p => p.Bohrprofile).AsQueryable();

#pragma warning disable CA1304 // Specify CultureInfo
        if (gemeinde != null)
        {
            standorte = standorte.Where(s => s.Gemeinde.ToLower().Contains(gemeinde.ToLower()));
        }

        if (!string.IsNullOrEmpty(gbnummer))
        {
            standorte = standorte.Where(s => s.GrundbuchNr.ToLower().Contains(gbnummer.ToLower()));
        }

        if (!string.IsNullOrEmpty(bezeichnung))
        {
            standorte = standorte.Where(s => s.Bezeichnung.ToLower().Contains(bezeichnung.ToLower()));
#pragma warning restore CA1304 // Specify CultureInfo
        }

        // Use universal time zone to convert time.
        TimeZoneInfo ut = TimeZoneInfo.Utc;

        if (erstellungsdatum != null)
        {
            standorte = standorte.Where(s => s.Erstellungsdatum != null && s.Erstellungsdatum!.Value.Date == TimeZoneInfo.ConvertTimeToUtc(erstellungsdatum.Value, ut).Date);
        }

        if (mutationsdatum != null)
        {
            standorte = standorte.Where(s => s.Mutationsdatum != null && s.Mutationsdatum!.Value.Date == TimeZoneInfo.ConvertTimeToUtc(mutationsdatum.Value, ut).Date);
        }

        return await standorte.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
