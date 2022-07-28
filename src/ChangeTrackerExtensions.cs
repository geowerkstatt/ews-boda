using EWS.Authentication;
using EWS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace EWS;

public static class ChangeTrackerExtensions
{
    private const string DefaultUserName = "ews-boda";

    internal static void UpdateChangeInformation(this ChangeTracker changeTracker, HttpContext? httpContext)
    {
        var entities = changeTracker.Entries<IDateUserSettable>();
        var userName = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.Erstellungsdatum = DateTime.Now;
                entity.Entity.UserErstellung = userName ?? DefaultUserName;
            }
            else
            {
                entity.Entity.Mutationsdatum = DateTime.Now;
                entity.Entity.UserMutation = userName ?? DefaultUserName;
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="Standort.AfuUser"/> and <see cref="Standort.AfuDatum"/>
    /// depending on whether the <see cref="Standort.FreigabeAfu"/> has been set or not.
    /// </summary>
    /// <param name="changeTracker"><see cref="ChangeTracker"/> which provides access to
    /// change tracking information.</param>
    /// <param name="httpContext"></param>
    internal static void UpdateFreigabeAfuFields(this ChangeTracker changeTracker, HttpContext? httpContext)
    {
        var userName = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        foreach (var entry in changeTracker.Entries<Standort>())
        {
            if (entry.Entity.FreigabeAfu)
            {
                entry.Entity.AfuDatum ??= DateTime.Now;
                entry.Entity.AfuUser ??= userName ?? DefaultUserName;
            }
            else
            {
                entry.Entity.AfuDatum = null;
                entry.Entity.AfuUser = null;
            }
        }
    }
}
