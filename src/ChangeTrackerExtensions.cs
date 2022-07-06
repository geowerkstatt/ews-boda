using EWS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EWS
{
    public static class ChangeTrackerExtensions
    {
        internal static void UpdateChangeInformation(this ChangeTracker changeTracker)
        {
            var entities = changeTracker.Entries<IDateUserSettable>();

            // Replace as soon as authentication is available.
            string currentUser = "Temporary Test User";
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.Erstellungsdatum = DateTime.Now.ToUniversalTime();
                    entity.Entity.UserErstellung = currentUser;
                }
                else
                {
                    entity.Entity.Mutationsdatum = DateTime.Now.ToUniversalTime();
                    entity.Entity.UserMutation = currentUser;
                }
            }
        }
    }
}
