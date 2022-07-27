using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class BohrungController : EwsControllerBase<Bohrung>
{
    private readonly HttpClient client;
    private readonly ILogger<DataServiceController> logger;
    private readonly EwsContext context;

    public BohrungController(HttpClient client, ILogger<DataServiceController> logger, EwsContext context)
        : base(context)
    {
        this.client = client;
        this.logger = logger;
        this.context = context;
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> CreateAsync(Bohrung entity)
    {
        entity.Bohrprofile = null;
        if (entity.Geometrie == null)
        {
            return await base.CreateAsync(entity).ConfigureAwait(false);
        }
        else
        {
            return await UpdateStandortAndBohrung(base.CreateAsync, entity).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> EditAsync(Bohrung entity)
    {
        entity.Bohrprofile = null;
        if (entity.Geometrie == null)
        {
            return await base.EditAsync(entity).ConfigureAwait(false);
        }
        else
        {
            return await UpdateStandortAndBohrung(base.EditAsync, entity).ConfigureAwait(false);
        }
    }

    private async Task<IActionResult> UpdateStandortAndBohrung(Func<Bohrung, Task<IActionResult>> createOrUpdateBohrung, Bohrung item)
    {
        var updateResult = await UpdateStandort(item).ConfigureAwait(false);

        // Case if Api call is not successful.
        if (updateResult?.Value == null)
        {
            var objectResult = updateResult.Result as ObjectResult;
            return (IActionResult)Task.FromResult(objectResult);
        }
        else
        {
            // Case if Api call is successful, but point is not in Kanton Solothurn
            if (string.IsNullOrEmpty(updateResult.Value.Gemeinde))
            {
                return Problem($"Call to data service Api did not yield any results. The supplied geometry '{item.Geometrie}' may not lie in Kanton Solothurn.");
            }

            return await createOrUpdateBohrung(item).ConfigureAwait(false);
        }
    }

    private async Task<ActionResult<DataServiceResponse>> UpdateStandort(Bohrung bohrung)
    {
        var standortToUpdate = Context.Standorte.Include(s => s.Bohrungen).SingleOrDefault(s => s.Id == bohrung.StandortId);

        var bohrungen = new List<Bohrung>();

        if (standortToUpdate.Bohrungen != null)
        {
            bohrungen.AddRange(standortToUpdate.Bohrungen.ToList());
        }

        // If no primary key is present in the Bohrung it was newly added.
        // Otherwise it is being edited and the geometry of the existing Bohrung needs to be replaced for the Dataservice Api call.
        if (bohrung.Id == 0)
            bohrungen.Add(bohrung);
        else
            bohrungen.Find(b => b.Id == bohrung.Id).Geometrie = bohrung.Geometrie;

        var controller = new DataServiceController(client, logger, context);
        var response = await controller.GetAsync(bohrungen.Select(b => b.Geometrie).ToList()).ConfigureAwait(false);

        if (response.Value != null && response.Value.Gemeinde != null)
        {
            var dataServiceResponse = response.Value;
            standortToUpdate.Gemeinde = dataServiceResponse.Gemeinde;
            standortToUpdate.GrundbuchNr = dataServiceResponse.Grundbuchnummer;
            Context.Standorte.Update(standortToUpdate);
            Context.SaveChanges();
        }

        return response;
    }

    /// <summary>
    /// Asynchronously gets the <see cref="Bohrung"/> for the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The bohrung id.</param>
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Bohrung>> GetByIdAsync(int id)
    {
        var bohrung = await Context.Bohrungen
            .Include(b => b.Bohrprofile).ThenInclude(b => b.Schichten).ThenInclude(s => s.CodeSchicht)
            .Include(b => b.Bohrprofile).ThenInclude(b => b.Vorkommnisse).ThenInclude(v => v.Typ)
            .SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);

        if (bohrung == null)
        {
            return NotFound();
        }
        else
        {
            return bohrung;
        }
    }
}
