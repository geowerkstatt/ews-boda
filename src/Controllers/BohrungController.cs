using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class BohrungController : ControllerBase
{
    private readonly EwsContext context;
    public BohrungController(EwsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BohrungDTO>>> Get()
    {
        return await context.Bohrungen.Take(20).Select(x => BohrungToDTO(x)).ToListAsync().ConfigureAwait(false);
    }

    private static BohrungDTO BohrungToDTO(Bohrung bohrung)
    {
        return new BohrungDTO
        {
            Id = bohrung.Id,
            StandortId = bohrung.StandortId,
            Bezeichnung = bohrung.Bezeichnung,
            Geometrie = new Point((int)bohrung.Geometrie.X, (int)bohrung.Geometrie.Y),
        };
    }
}
