using Microsoft.AspNetCore.Mvc;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    public string Get() => "0.1.0";
}
