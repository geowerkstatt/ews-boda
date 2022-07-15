namespace EWS.Authentication
{
    /// <summary>
    /// Extension methods used to expose middlewares through <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="CheckAuthorizedMiddleware"/> to the specified
        /// <see cref="IApplicationBuilder"/>, which checks whether the user is authorized
        /// to use the ews-boda application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to app after the operation has completed.</returns>
        public static IApplicationBuilder UseCheckAuthorizedMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware<CheckAuthorizedMiddleware>();
    }
}
