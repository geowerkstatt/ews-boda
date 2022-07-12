using EWS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class BohrungController : EwsControllerBase<Bohrung>
{
    public BohrungController(EwsContext context)
        : base(context)
    {
    }
}
