using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[Authorize]
[ApiController]
[Route("[controller]")]
public class BohrungController : EwsControllerBase<Bohrung>
{
    public BohrungController(EwsContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(Bohrung item)
    {
        item.Bohrprofile = null;
        return base.CreateAsync(item);
    }

    /// <inheritdoc/>
    public override Task<IActionResult> EditAsync(Bohrung item)
    {
        item.Bohrprofile = null;
        return base.EditAsync(item);
    }
}
