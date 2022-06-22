using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        [FromQuery] int? gemeindenummer = null,
        [FromQuery] string? gbnummer = null,
        [FromQuery] string? bezeichnung = null,
        [FromQuery] string? erstellungsdatum = null,
        [FromQuery] string? mutationsdatum = null)
    {
        var standorte = context.Standorte.AsQueryable();
        if (gemeindenummer != null)
        {
            standorte = standorte.Where(s => s.Gemeinde == gemeindenummer);
        }

        if (gbnummer != null)
        {
            standorte = standorte.Where(s => s.GrundbuchNr == gbnummer);
        }

        if (bezeichnung != null)
        {
            standorte = standorte.Where(s => s.Bezeichnung == bezeichnung);
        }

        var cultureInfo = new CultureInfo("de_CH", false);

        if (erstellungsdatum != null)
        {
            standorte = standorte.Where(s => s.Erstellungsdatum == DateTime.Parse(erstellungsdatum, cultureInfo));
        }

        if (mutationsdatum != null)
        {
            standorte = standorte.Where(s => s.Mutationsdatum == DateTime.Parse(mutationsdatum, cultureInfo));
        }

        return await standorte.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
