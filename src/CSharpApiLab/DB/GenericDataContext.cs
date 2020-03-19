using Microsoft.EntityFrameworkCore;

namespace CSharpApiLab.DB
{
    public class GenericDataContext : DbContext
    {
        public GenericDataContext(DbContextOptions<GenericDataContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Multiple private key

            modelBuilder.Entity<Server>().HasKey(u => new { u.UserId, u.ID });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers { get; set; }
    }
}
