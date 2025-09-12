using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<TestCode> TestCode { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profiles>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}