using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class UserController : EwsControllerBase<User>
{
    public UserController(EwsContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets the current authenticated and authorized ews-boda user.
    /// </summary>
    [Authorize(Policy = Policies.IsExtern)]
    [HttpGet("self")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "HTTP method attributes cannot be used on properties.")]
    public ActionResult<User?> GetUserInformation()
    {
        var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Context.Users.SingleOrDefault(x => x.Name == userName);
    }

    /// <summary>
    /// Asynchronously gets all the users available.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAsync() =>
        await Context.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(User entity) =>
        Task.FromResult((IActionResult)BadRequest("Creating new users is not supported."));
}
