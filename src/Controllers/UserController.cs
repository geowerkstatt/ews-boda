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
    public UserController(EwsContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Asynchronously gets all the users available.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<User>> GetAsync() =>
        await Context.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);

    /// <inheritdoc/>
    public override Task<IActionResult> CreateAsync(User item) =>
        Task.FromResult((IActionResult)BadRequest("Creating new users is not supported."));
}
