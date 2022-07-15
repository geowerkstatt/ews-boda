using EWS.Models;

namespace EWS.Authentication
{
    /// <summary>
    /// Checks whether the user is authorized to use the ews-boda application.
    /// </summary>
    public class CheckAuthorizedMiddleware
    {
        private const string AuthorizedGroupName = "GA_Xen_EWS_Boda";
        private const string GroupClaimType = "groups";
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckAuthorizedMiddleware"/> class.
        /// </summary>
        /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
        public CheckAuthorizedMiddleware(RequestDelegate next) => this.next = next;

        /// <summary>
        /// Request handling method.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <param name="logger">The logger for this instance.</param>
        /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
        public async Task InvokeAsync(HttpContext httpContext, ILogger<CheckAuthorizedMiddleware> logger)
        {
            var authorized = httpContext.User.Claims.FirstOrDefault(x => x.Type == GroupClaimType && x.Value == AuthorizedGroupName);
            if (authorized == null)
            {
                var errorMessage = $"The user is not authorized to use this application. Group name <{AuthorizedGroupName}> not found in claim <{GroupClaimType}>.";
                logger.LogError(errorMessage);
                await httpContext.Response.WriteProblemDetailsAsync(
                    "Authorization Exception", errorMessage, StatusCodes.Status403Forbidden).ConfigureAwait(false);

                return;
            }

            // Call the next delegate/middleware in the pipeline.
            await next(httpContext).ConfigureAwait(false);
        }
    }
}
