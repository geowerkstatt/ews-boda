using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EWS;

/// <summary>
/// Test helper methods.
/// </summary>
internal static class Helpers
{
    /// <summary>
    /// Gets a <see cref="ControllerContext"/> with pre-configured<see cref="ClaimTypes.NameIdentifier"/> and
    /// <see cref="ClaimTypes.Role"/> claims.
    /// </summary>
    internal static ControllerContext GetControllerContext(UserRole userRole = UserRole.Administrator)
    {
        var httpContext = new DefaultHttpContext();
        var identity = (ClaimsIdentity)httpContext.User.Identity!;

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "PEEVEDSOUFFLE"));
        identity.AddClaim(new Claim(ClaimTypes.Role, userRole.ToString()));

        return new() { HttpContext = httpContext };
    }
}
