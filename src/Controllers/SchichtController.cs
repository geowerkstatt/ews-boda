using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class SchichtController : EwsControllerBase<Schicht>
{
    public SchichtController(EwsContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(Schicht entity)
    {
        return base.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public override Task<IActionResult> EditAsync(Schicht entity)
    {
        return base.EditAsync(entity);
    }
}
