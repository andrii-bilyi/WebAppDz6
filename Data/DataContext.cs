using Microsoft.EntityFrameworkCore;

namespace WebAppDz4.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }       

        public DataContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("WebAppDz4");
        }
    }
}
