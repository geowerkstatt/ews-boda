using EWS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

/// <summary>
/// Controller base class which provides some basic actions
/// to create, update and delete items.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
[ApiController]
[Route("[controller]")]
public class EwsControllerBase<TModel> : ControllerBase
    where TModel : EwsModelBase, new()
{
    public EwsContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the <typeparamref name="TModel"/> class.
    /// </summary>
    /// <param name="context">The EF database context containing data for the EWS-Boda application.</param>
    public EwsControllerBase(EwsContext context)
    {
        Context = context;
        Context.Set<TModel>();
    }

    /// <summary>
    /// Asynchronously creates the <paramref name="item"/> specified.
    /// </summary>
    /// <param name="item">The item to create.</param>
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync(TModel item)
    {
        await Context.AddAsync(item).ConfigureAwait(false);
        return await SaveChangesAsync(() => CreatedAtAction(nameof(TModel), item)).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously updates the <paramref name="item"/> specified.
    /// </summary>
    /// <param name="item">The item to update.</param>
    [HttpPut]
    public virtual async Task<IActionResult> EditAsync(TModel item)
    {
        var itemToEdit = await Context.FindAsync(typeof(TModel), item.Id).ConfigureAwait(false);
        if (itemToEdit == null)
        {
            return NotFound();
        }
        else
        {
            Context.Entry(itemToEdit).CurrentValues.SetValues(item);
            return await SaveChangesAsync(() => Ok()).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously deletes the item with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The item id to delete.</param>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync(int id)
    {
        var itemToDelete = await Context.FindAsync(typeof(TModel), id).ConfigureAwait(false);
        if (itemToDelete == null)
        {
            return NotFound();
        }
        else
        {
            Context.Remove(itemToDelete);
            return await SaveChangesAsync(() => Ok()).ConfigureAwait(false);
        }
    }

    private async Task<IActionResult> SaveChangesAsync(Func<IActionResult> successResult)
    {
        try
        {
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return successResult();
        }
        catch (Exception)
        {
            return Problem(
                "An error occurred while saving the entity changes.",
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
