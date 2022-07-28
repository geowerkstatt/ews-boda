using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class BohrprofilController : EwsControllerBase<Bohrprofil>
{
    public BohrprofilController(EwsContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(Bohrprofil entity)
    {
        entity.Schichten = null;
        entity.Vorkommnisse = null;
        return base.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public override Task<IActionResult> EditAsync(Bohrprofil entity)
    {
        entity.Schichten = null;
        entity.Vorkommnisse = null;
        return base.EditAsync(entity);
    }

    /// <summary>
    /// Asynchronously gets the <see cref="Bohrprofil"/> for the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The bohrprofil id.</param>
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Bohrprofil>> GetByIdAsync(int id)
    {
        var bohrprofil = await Context.Bohrprofile
            .Include(b => b.Schichten).ThenInclude(s => s.CodeSchicht)
            .Include(b => b.Vorkommnisse).ThenInclude(v => v.Typ)
            .SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);

        if (bohrprofil == null)
        {
            return NotFound();
        }
        else
        {
            return bohrprofil;
        }
    }
}
