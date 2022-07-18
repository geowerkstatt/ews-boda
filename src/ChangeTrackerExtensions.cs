using EWS.Authentication;
using EWS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EWS
{
    public static class ChangeTrackerExtensions
    {
        internal static void UpdateChangeInformation(this ChangeTracker changeTracker, UserContext userContext)
        {
            var entities = changeTracker.Entries<IDateUserSettable>();
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.Erstellungsdatum = DateTime.Now;
                    entity.Entity.UserErstellung = userContext.CurrentUser.Name;
                }
                else
                {
                    entity.Entity.Mutationsdatum = DateTime.Now;
                    entity.Entity.UserMutation = userContext.CurrentUser.Name;
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="Standort.AfuUser"/> and <see cref="Standort.AfuDatum"/>
        /// depending on whether the <see cref="Standort.FreigabeAfu"/> has been set or not.
        /// </summary>
        /// <param name="changeTracker"><see cref="ChangeTracker"/> which provides access to
        /// change tracking information.</param>
        /// <param name="userContext">The context for the current user.</param>
        internal static void UpdateFreigabeAfuFields(this ChangeTracker changeTracker, UserContext userContext)
        {
            foreach (var entry in changeTracker.Entries<Standort>())
            {
                if (entry.Entity.FreigabeAfu)
                {
                    entry.Entity.AfuDatum ??= DateTime.Now;
                    entry.Entity.AfuUser ??= userContext.CurrentUser.Name;
                }
                else
                {
                    entry.Entity.AfuDatum = null;
                    entry.Entity.AfuUser = null;
                }
            }
        }
    }
}
