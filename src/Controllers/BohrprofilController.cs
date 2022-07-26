using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
