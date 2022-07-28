using EWS.Models;
using System.Security.Claims;

namespace EWS.Authentication;

/// <summary>
/// Automatically registers authenticated and authorized ews-boda users.
/// </summary>
public class AutoUserRegistrationMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoUserRegistrationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    public AutoUserRegistrationMiddleware(RequestDelegate next) => this.next = next;

    /// <summary>
    /// Request handling method.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="dbContext">The EF database context containing data for the EWS-Boda application.</param>
    /// <param name="logger">The logger for this instance.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "LINQ to SQL does not support StringComparison. The current configuration is case-sensitive by default.")]
    public async Task InvokeAsync(HttpContext httpContext, EwsContext dbContext, ILogger<AutoUserRegistrationMiddleware> logger)
    {
        var userName = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userName))
        {
            var errorMessage = $"The given user name <{userName}> in claim <sub> is not valid";
            logger.LogError(errorMessage);
            await httpContext.Response.WriteProblemDetailsAsync(
                "Authorization Exception", errorMessage, StatusCodes.Status403Forbidden).ConfigureAwait(false);

            return;
        }

        var user = dbContext.Users.SingleOrDefault(x => x.Name.Equals(userName));
        if (user == null)
        {
            user = dbContext.Users.Add(new User { Name = userName, Role = UserRole.Extern }).Entity;

            try
            {
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                var errorMessage = $"Error while saving user <{userName}> to the database.";
                logger.LogError(errorMessage);
                await httpContext.Response.WriteProblemDetailsAsync(
                    "Database error", errorMessage, StatusCodes.Status400BadRequest).ConfigureAwait(false);

                return;
            }
        }

        (httpContext.User.Identity as ClaimsIdentity).AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

        // Call the next delegate/middleware in the pipeline.
        await next(httpContext).ConfigureAwait(false);
    }
}
