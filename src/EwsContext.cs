using EWS.Models;
using Microsoft.EntityFrameworkCore;

namespace EWS
{
    /// <summary>
    /// The EF database context containing data for the EWS-Boda application.
    /// </summary>
    public class EwsContext : DbContext
    {
        public DbSet<Bohrprofil> Bohrprofile { get; set; }
        public DbSet<Bohrung> Bohrungen { get; set; }
        public DbSet<Code> Codes { get; set; }
        public DbSet<CodeSchicht> CodeSchichten { get; set; }
        public DbSet<CodeTyp> CodeTypen { get; set; }
        public DbSet<Schicht> Schichten { get; set; }
        public DbSet<Standort> Standorte { get; set; }
        public DbSet<Vorkommnis> Vorkommnisse { get; set; }
        public DbSet<User> Users { get; set; }

        public EwsContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("bohrung");
            modelBuilder.HasPostgresExtension("postgis");
            base.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            ChangeTracker.UpdateChangeInformation();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.UpdateChangeInformation();
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
}
