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
    }
}
