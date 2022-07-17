using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[Authorize(Policy = Policies.IsExtern)]
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
}
