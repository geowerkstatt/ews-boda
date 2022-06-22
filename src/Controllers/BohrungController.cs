using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class BohrungController : ControllerBase
{
    private readonly EwsContext context;
    public BohrungController(EwsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Bohrung>> GetAsync(
        [FromQuery] int? gemeindenummer = null,
        [FromQuery] string? gbnummer = null,
        [FromQuery] string? bezeichnung = null,
        [FromQuery] string? erstellungsdatum = null,
        [FromQuery] string? mutationsdatum = null)
    {
        var bohrungen = context.Bohrungen.AsQueryable();
        if (gemeindenummer != null)
        {
            bohrungen = bohrungen.Where(b => context.Standorte
                .Where(s => s.Gemeinde == gemeindenummer)
                .Select(s => s.Id)
                .Contains(b.StandortId));
        }

        if (gbnummer != null)
        {
            bohrungen = bohrungen.Where(b => context.Standorte
                .Where(s => s.GrundbuchNr == gbnummer)
                .Select(s => s.Id)
                .Contains(b.StandortId));
        }

        if (bezeichnung != null)
        {
            bohrungen = bohrungen.Where(b => context.Standorte
                .Where(s => s.Bezeichnung == bezeichnung)
                .Select(s => s.Id)
                .Contains(b.StandortId));
        }

        var cultureInfo = new CultureInfo("de_CH", false);

        if (erstellungsdatum != null)
        {
            bohrungen = bohrungen.Where(b => context.Standorte
                .Where(s => s.Erstellungsdatum == DateTime.Parse(erstellungsdatum, cultureInfo))
                .Select(s => s.Id)
                .Contains(b.StandortId));
        }

        if (mutationsdatum != null)
        {
            bohrungen = bohrungen.Where(b => context.Standorte
                .Where(s => s.Mutationsdatum == DateTime.Parse(mutationsdatum, cultureInfo))
                .Select(s => s.Id)
                .Contains(b.StandortId));
        }

        return await bohrungen.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
