using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EWS.Authentication
{
    /// <summary>
    /// Authentication extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Asynchronously flushes a <see cref="ProblemDetails"/> object to the given <paramref name="httpResponse"/>.
        /// </summary>
        /// <param name="httpResponse">The outgoing side of an individual HTTP request.</param>
        /// <param name="title">A short summary of the problem type.</param>
        /// <param name="detail">An explanation specific to this occurrence of the problem.</param>
        /// <param name="status">The HTTP status code.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the execution of this write operation.</returns>
        public static async Task WriteProblemDetailsAsync(this HttpResponse httpResponse, string title, string detail, int status, CancellationToken cancellationToken = default)
        {
            var result = JsonSerializer.Serialize(new ProblemDetails() { Title = title, Detail = detail, Status = status });
            httpResponse.StatusCode = status;
            httpResponse.ContentType = "application/json; charset=UTF-8";
            await httpResponse.WriteAsync(result, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            await httpResponse.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public static string? GetUserName(this IEnumerable<Claim> claims) =>
            claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }
}
