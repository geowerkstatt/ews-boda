using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class BohrungController : EwsControllerBase<Bohrung>
{
    public BohrungController(EwsContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(Bohrung entity)
    {
        entity.Bohrprofile = null;
        return base.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public override Task<IActionResult> EditAsync(Bohrung entity)
    {
        entity.Bohrprofile = null;
        return base.EditAsync(entity);
    }

    /// <summary>
    /// Asynchronously gets the <see cref="Bohrung"/> for the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The standort id.</param>
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var bohrung = await Context.Bohrungen
            .Include(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.CodeSchicht)
            .Include(b => b.Bohrprofile).ThenInclude(b => b.Vorkommnisse)
            .SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);

        if (bohrung == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(bohrung);
        }
    }
}
