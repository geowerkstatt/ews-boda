using EWS.Authentication;
using EWS.Models;

namespace EWS
{
    /// <summary>
    /// Test helpers.
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Gets a default, pre-configured <see cref="UserContext"/>.
        /// </summary>
        internal static UserContext GetUserContext(UserRole userRole = UserRole.Administrator) =>
            new()
            {
                CurrentUser = new User
                {
                    Name = "PEEVEDSOUFFLE",
                    Role = userRole,
                },
            };
    }
}
