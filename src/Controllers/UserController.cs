using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : EwsControllerBase<User>
{
    private readonly UserContext userContext;

    public UserController(EwsContext context, UserContext userContext)
        : base(context)
    {
        this.userContext = userContext;
    }

    /// <summary>
    /// Gets the current authenticated and authorized ews-boda user.
    /// </summary>
    [HttpGet("self")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "HTTP method attributes cannot be used on properties.")]
    public ActionResult<User?> GetUserInformation() => userContext.CurrentUser;

    /// <summary>
    /// Asynchronously gets all the users available.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<User>> GetAsync() =>
        await Context.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(User entity) =>
        Task.FromResult((IActionResult)BadRequest("Creating new users is not supported."));
}
