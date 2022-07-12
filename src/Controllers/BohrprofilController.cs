using EWS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class BohrprofilController : EwsControllerBase<Bohrprofil>
{
    public BohrprofilController(EwsContext context)
        : base(context)
    {
    }
}
