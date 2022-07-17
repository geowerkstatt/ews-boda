using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EWS
{
    /// <summary>
    /// Test helpers methods.
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Gets a <see cref="ControllerContext"/> with pre-configured <see cref="ClaimTypes.NameIdentifier"/> and
        /// <see cref="ClaimTypes.Role"/> claims/>.
        /// </summary>
        internal static ControllerContext GetControllerContext(UserRole userRole = UserRole.Administrator)
        {
            var httpContext = new DefaultHttpContext();
            (httpContext.User.Identity as ClaimsIdentity).AddClaim(new Claim(ClaimTypes.NameIdentifier, "PEEVEDSOUFFLE"));
            (httpContext.User.Identity as ClaimsIdentity).AddClaim(new Claim(ClaimTypes.Role, userRole.ToString()));
            return new ControllerContext() { HttpContext = httpContext };
        }
    }
}
