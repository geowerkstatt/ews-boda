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

    /// <summary>
    /// Asynchronously updates the <paramref name="user"/> specified.
    /// </summary>
    /// <param name="user">The <see cref="User"/> to update.</param>
    [HttpPut]
    public async Task<IActionResult> EditAsync(User user)
    {
        var userToEdit = context.Users.SingleOrDefault(x => x.Id == user.Id);
        if (userToEdit == null)
        {
            return NotFound();
        }
        else
        {
            context.Entry(userToEdit).CurrentValues.SetValues(user);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }

    /// <summary>
    /// Asynchronously deletes the <see cref="User"/> with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The <see cref="User"/> id to delete.</param>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var userToDelete = context.Users.SingleOrDefault(x => x.Id == id);
        if (userToDelete == null)
        {
            return NotFound();
        }
        else
        {
            context.Remove(userToDelete);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}
