using Microsoft.EntityFrameworkCore;

namespace HotChocolatey.Model.Save
{
    public class ListContext : DbContext
    {
        private readonly string filename;

        public ListContext(string filename)
        {
            this.filename = filename;
        }

        public ListContext()
            : this("test.sqlite")
        {
            // Used for Nuget-Package-Console commands
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data source={filename}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstalledPackage>()
                .HasKey(c => new { c.Id, c.WorkStationId });
        }

        public DbSet<WorkStation> WorkStations { get; set; }
        public DbSet<InstalledPackage> InstalledPackages { get; set; }
    }
}
