using EWS.Models;

namespace EWS.Authentication
{
    /// <summary>
    /// The context for the current user. Provides information for the current logged-in <see cref="CurrentUser"/>.
    /// </summary>
    public class UserContext
    {
        /// <summary>
        /// Gets or sets the current logged-in user.
        /// </summary>
        public User? CurrentUser { get; set; }
    }
}
