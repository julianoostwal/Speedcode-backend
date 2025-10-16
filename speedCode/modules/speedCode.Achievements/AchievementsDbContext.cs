using Microsoft.EntityFrameworkCore;

namespace speedCode.Achievements
{
    public class AchievementDbContext : DbContext
    {
        public DbSet<Achievement> Achievements { get; set; }

        public AchievementDbContext(DbContextOptions<AchievementDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Achievement>()
                .Property(a => a.id)
                .HasDefaultValueSql("gen_random_uuid()");
        }
    }
}
