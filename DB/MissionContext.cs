using AlignAPI.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlignAPI.DB
{
    public class MissionContext : DbContext
    {
        public MissionContext(DbContextOptions<MissionContext> options) : base(options)
        {
        }

        public DbSet<Mission> Missions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<Mission>(entity =>
            {
                entity.Property(e => e.Location).HasColumnType("geometry(Point)");
            });
        }
    }
}
