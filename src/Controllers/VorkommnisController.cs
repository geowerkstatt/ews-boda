using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[Authorize(Policy = PolicyNames.Extern)]
[ApiController]
[Route("[controller]")]
public class VorkommnisController : EwsControllerBase<Vorkommnis>
{
    public VorkommnisController(EwsContext context)
        : base(context)
    {
    }
}
