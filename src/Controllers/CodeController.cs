using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class CodeController : EwsControllerBase<Code>
{
    public CodeController(EwsContext context)
    : base(context)
    {
    }

    [HttpGet]
    public async Task<IEnumerable<Code>> GetByCodeTypAsync(int codeTypId)
    {
        return await Context.Codes.Where(c => c.CodetypId == codeTypId).Include(c => c.Codetyp).AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
