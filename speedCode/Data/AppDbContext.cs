using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<TestCode> TestCode { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<RoomUser> RoomUser { get; set; }
        public DbSet<RoomExercise> RoomExercise { get; set; }
        public DbSet<ExerciseTest> ExerciseTest { get; set; }
        public DbSet<ProfileExercise> ProfileExercise { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<ExerciseTest>()
                .HasKey(et => new { et.exerciseId, et.testId });

            modelBuilder.Entity<ExerciseTest>()
                .HasOne(et => et.exercise)
                .WithMany(e => e.ExerciseTests)
                .HasForeignKey(et => et.exerciseId);

            modelBuilder.Entity<ExerciseTest>()
                .HasOne(et => et.test)
                .WithMany(t => t.ExerciseTests)
                .HasForeignKey(et => et.testId);
            
            modelBuilder.Entity<Profiles>()
                .Property(p => p.createdAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TestCode>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Rooms>()
                .Property(e => e.roomLevel)
                .HasConversion<string>();
        }
    }
}