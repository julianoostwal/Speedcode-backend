using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<TestCode> TestCode { get; set; }
        public DbSet<Rooms> Room { get; set; }
        public DbSet<RoomUser> RoomUser { get; set; }
        public DbSet<RoomExercise> RoomExercise { get; set; }
        public DbSet<ExerciseTest> ExerciseTest { get; set; }

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