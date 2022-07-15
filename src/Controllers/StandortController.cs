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
        var standorte = GetAll();
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

    /// <summary>
    /// Asynchronously gets the <see cref="Standort"/> for the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The standort id.</param>
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var standort = await GetAll().SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
        if (standort == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(standort);
        }
    }

    private IQueryable<Standort> GetAll()
    {
        return Context.Standorte
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).AsQueryable();
    }
}
