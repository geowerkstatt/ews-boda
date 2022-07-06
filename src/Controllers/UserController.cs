using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly EwsContext context;

    public UserController(EwsContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Asynchronously gets all the users available.
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<User>> GetAsync() =>
        await context.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);
}
