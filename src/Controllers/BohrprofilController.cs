using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[Authorize]
[ApiController]
[Route("[controller]")]
public class BohrprofilController : EwsControllerBase<Bohrprofil>
{
    public BohrprofilController(EwsContext context)
        : base(context)
    {
    }
}
