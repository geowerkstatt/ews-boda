using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class CodeSchichtController : EwsControllerBase<Code>
{
    public CodeSchichtController(EwsContext context)
    : base(context)
    {
    }

    [HttpGet]
    public async Task<IEnumerable<CodeSchicht>> GetAsync()
    {
        return await Context.CodeSchichten.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }
}
