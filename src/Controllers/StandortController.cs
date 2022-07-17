using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[Authorize(Policy = Policies.IsExtern)]
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

        if (!string.IsNullOrEmpty(gemeinde))
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

        if (erstellungsdatum != null)
        {
            standorte = standorte.Where(s => s.Erstellungsdatum != null && s.Erstellungsdatum!.Value.Date == erstellungsdatum.Value.Date);
        }

        if (mutationsdatum != null)
        {
            standorte = standorte.Where(s => s.Mutationsdatum != null && s.Mutationsdatum!.Value.Date == mutationsdatum.Value.Date);
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

    /// <inheritdoc/>
    public override async Task<IActionResult> CreateAsync(Standort entity)
    {
        entity.Bohrungen = null;
        return await base.CreateAsync(entity).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> EditAsync(Standort entity)
    {
        var entityToEdit = await Context.FindAsync<Standort>(entity.Id).ConfigureAwait(false);
        var isExtern = HttpContext.User.IsInRole(UserRole.Extern.ToString());
        if ((entity.FreigabeAfu || entityToEdit?.FreigabeAfu == true) && isExtern)
        {
            return Problem(
                title: "Authorization Exception",
                detail: "User with role <Extern> are not allowed to set <Freigabe AfU> or edit Standort with <Freigabe AfU>.",
                statusCode: StatusCodes.Status403Forbidden);
        }

        entity.Bohrungen = null;
        return await base.EditAsync(entity).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        var entityToDelete = await Context.FindAsync<Standort>(id).ConfigureAwait(false);
        var isExtern = HttpContext.User.IsInRole(UserRole.Extern.ToString());
        if (entityToDelete?.FreigabeAfu == true && isExtern)
        {
            return Problem(
                title: "Authorization Exception",
                detail: "User with role <Extern> are not allowed to delete Standort with <Freigabe AfU>.",
                statusCode: StatusCodes.Status403Forbidden);
        }

        return await base.DeleteAsync(id).ConfigureAwait(false);
    }

    private IQueryable<Standort> GetAll()
    {
        return Context.Standorte
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Schichten)
            .Include(s => s.Bohrungen).ThenInclude(b => b.Bohrprofile).ThenInclude(b => b.Vorkomnisse).AsQueryable();
    }
}
