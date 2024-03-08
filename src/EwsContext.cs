using EWS.Authentication;
using EWS.Models;
using Microsoft.EntityFrameworkCore;

namespace EWS;

/// <summary>
/// The EF database context containing data for the EWS-Boda application.
/// </summary>
public class EwsContext : DbContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public DbSet<Bohrprofil> Bohrprofile { get; set; }
    public DbSet<Bohrung> Bohrungen { get; set; }
    public DbSet<Code> Codes { get; set; }
    public DbSet<CodeSchicht> CodeSchichten { get; set; }
    public DbSet<CodeTyp> CodeTypen { get; set; }
    public DbSet<Schicht> Schichten { get; set; }
    public DbSet<Standort> Standorte { get; set; }
    public DbSet<Vorkommnis> Vorkommnisse { get; set; }
    public DbSet<User> Users { get; set; }

    public EwsContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bohrung");
        modelBuilder.HasPostgresExtension("postgis");
    }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        ChangeTracker.UpdateChangeInformation(httpContextAccessor.HttpContext);
        ChangeTracker.UpdateFreigabeAfuFields(httpContextAccessor.HttpContext);
        return base.SaveChanges();
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.UpdateChangeInformation(httpContextAccessor.HttpContext);
        ChangeTracker.UpdateFreigabeAfuFields(httpContextAccessor.HttpContext);
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows to call <see cref="SaveChanges" /> without updating the change information like dates and user names.
    /// </summary>
    internal int SaveChangesWithoutUpdatingChangeInformation()
    {
        return base.SaveChanges();
    }
}
