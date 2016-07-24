using System.Data.Entity;
using System.Data.SQLite;
using SQLite.CodeFirst;

namespace HotChocolatey.Model.Save
{
    public class ListContext : DbContext
    {
        public ListContext(string filename) : base(
            new SQLiteConnection
            {
                ConnectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = filename,
                    ForeignKeys = true
                }.ConnectionString
            }, 
            true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<ListContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public DbSet<WorkStation> WorkStations { get; set; }
    }
}
