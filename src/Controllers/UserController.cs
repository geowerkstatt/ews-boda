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
    [Authorize(Policy = PolicyNames.Extern)]
    [HttpGet("self")]
    public ActionResult<User?> GetUserInformation()
    {
        var userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Context.Users.SingleOrDefault(x => x.Name == userName);
    }

    /// <summary>
    /// Asynchronously gets all users.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAsync() =>
        await Context.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(User entity) =>
        Task.FromResult((IActionResult)BadRequest("Creating new users is not supported."));
}
