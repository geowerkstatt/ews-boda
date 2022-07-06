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
        var standorte = context.Standorte.Include(s => s.Bohrungen).ThenInclude(p => p.Bohrprofile).AsQueryable();
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
            standorte = standorte.Where(s => s.Erstellungsdatum != null && s.Erstellungsdatum!.Value.Date == TimeZoneInfo.ConvertTimeToUtc(erstellungsdatum.Value, ut).Date);
        }

        if (mutationsdatum != null)
        {
            standorte = standorte.Where(s => s.Mutationsdatum != null && s.Mutationsdatum!.Value.Date == TimeZoneInfo.ConvertTimeToUtc(mutationsdatum.Value, ut).Date);
        }

        return await standorte.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }

    [HttpPost]
    public IActionResult Create(Standort standort)
    {
        context.Standorte.Add(standort);
        context.SaveChanges();
        return CreatedAtAction(nameof(Standort), standort);
    }

    [HttpPut]
    public IActionResult Edit(Standort standort)
    {
        var standortToEdit = context.Standorte.SingleOrDefault(x => x.Id == standort.Id);
        if (standortToEdit == null)
        {
            return NotFound();
        }
        else
        {
            context.Entry(standortToEdit).CurrentValues.SetValues(standort);
            context.SaveChanges();
            return Ok();
        }
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var standortToDelete = context.Standorte.SingleOrDefault(x => x.Id == id);
        if (standortToDelete == null)
        {
            return NotFound();
        }
        else
        {
            context.Remove(standortToDelete);
            context.SaveChanges();
            return Ok();
        }
    }
}
