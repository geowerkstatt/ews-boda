using EWS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

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
