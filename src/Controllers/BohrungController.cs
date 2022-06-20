using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<IEnumerable<Bohrung>>> GetAsync()
    {
        return await context.Bohrungen.Select(x => x).ToListAsync().ConfigureAwait(false);
    }
}
