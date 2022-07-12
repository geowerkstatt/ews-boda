using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class CodeController : ControllerBase
{
    private readonly EwsContext context;
    public CodeController(EwsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Code>> GetByCodeTypAsync(int codeTypId)
    {
        return await context.Codes.Where(c => c.CodetypId == codeTypId).Include(c => c.Codetyp).AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
