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
    public async Task<IEnumerable<Standort>> GetAsync(int? gemeindenummer = null, string? gbnummer = null, string? bezeichnung = null, DateTime? erstellungsdatum = null, DateTime? mutationsdatum = null)
    {
        var standorte = Context.Standorte
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HTektonik)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HQualitaet).ThenInclude(b => b.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HFormationFels)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HFormationEndtiefe)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.Qualitaet)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.CodeSchicht)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.HQualitaet)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).ThenInclude(v => v.HTyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).ThenInclude(v => v.HQualitaet).ThenInclude(h => h.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Ablenkung).ThenInclude(a => a.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Qualitaet).ThenInclude(q => q.Codetyp)
            .AsQueryable();

        if (gemeindenummer != null)
        {
            standorte = standorte.Where(s => s.Gemeinde == gemeindenummer);
        }
#pragma warning disable CA1304 // Specify CultureInfo

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
    public async Task<IActionResult> GetAsync(int id)
    {
        var standort = await Context.Standorte
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HTektonik)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HQualitaet).ThenInclude(b => b.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HFormationFels)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.HFormationEndtiefe)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.Qualitaet)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.CodeSchicht)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.HQualitaet)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).ThenInclude(v => v.HTyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).ThenInclude(v => v.HQualitaet).ThenInclude(h => h.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Ablenkung).ThenInclude(a => a.Codetyp)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Qualitaet).ThenInclude(q => q.Codetyp).SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
        if (standort == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(standort);
        }
    }
}
