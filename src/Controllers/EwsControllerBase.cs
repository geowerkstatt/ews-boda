using EWS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

/// <summary>
/// Controller base class which provides some basic actions
/// to create, update and delete entities.
/// </summary>
/// <typeparam name="TEntity">The model type.</typeparam>
[ApiController]
[Route("[controller]")]
public class EwsControllerBase<TEntity> : ControllerBase
    where TEntity : EwsModelBase, new()
{
    private readonly ILogger<TEntity>? logger;

    public EwsContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the <typeparamref name="TEntity"/> class.
    /// </summary>
    /// <param name="logger">The logger for this instance.</param>
    protected EwsControllerBase(ILogger<TEntity> logger) => this.logger = logger;

    /// <summary>
    /// Initializes a new instance of the <typeparamref name="TEntity"/> class.
    /// </summary>
    /// <param name="context">The EF database context containing data for the EWS-Boda application.</param>
    public EwsControllerBase(EwsContext context)
    {
        Context = context;
        Context.Set<TEntity>();
    }

    /// <summary>
    /// Asynchronously creates the <paramref name="entity"/> specified.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync(TEntity entity)
    {
        await Context.AddAsync(entity).ConfigureAwait(false);
        return await SaveChangesAsync(() => CreatedAtAction(nameof(TEntity), entity)).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously updates the <paramref name="entity"/> specified.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    [HttpPut]
    public virtual async Task<IActionResult> EditAsync(TEntity entity)
    {
        var entityToEdit = await Context.FindAsync(typeof(TEntity), entity.Id).ConfigureAwait(false);
        if (entityToEdit == null)
        {
            return NotFound();
        }
        else
        {
            Context.Entry(entityToEdit).CurrentValues.SetValues(entity);
            return await SaveChangesAsync(() => Ok()).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously deletes the entity with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id of the entity to delete.</param>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync(int id)
    {
        var entityToDelete = await Context.FindAsync(typeof(TEntity), id).ConfigureAwait(false);
        if (entityToDelete == null)
        {
            return NotFound();
        }
        else
        {
            Context.Remove(entityToDelete);
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
        catch (Exception ex)
        {
            var errorMessage = "An error occurred while saving the entity changes.";
            logger?.LogError(ex, errorMessage);
            return Problem(errorMessage, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
