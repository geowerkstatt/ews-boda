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
            return await UpdateStandortBeforeContinouing(base.CreateAsync, entity).ConfigureAwait(false);
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
            return await UpdateStandortBeforeContinouing(base.EditAsync, entity).ConfigureAwait(false);
        }
    }

    private async Task<IActionResult> UpdateStandortBeforeContinouing(Func<Bohrung, Task<IActionResult>> operation, Bohrung item)
    {
        var updateResult = await UpdateStandort(item).ConfigureAwait(false);

        // Case if Api Call is not successful.
        if (updateResult != null && updateResult.Value == null)
        {
            var objectResult = updateResult.Result as ObjectResult;
            return (IActionResult)Task.FromResult(objectResult);
        }
        else
        {
            // Case if Api Call is successful but point is not in Kanton Solothurn
            if (string.IsNullOrEmpty(updateResult.Value.Gemeinde))
            {
                return BadRequest();
            }

            return await operation(item).ConfigureAwait(false);
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

        // If no primary key is not present in the Bohrung it was newly added.
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
}
