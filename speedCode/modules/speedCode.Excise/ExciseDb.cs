using speedCode.Excise.model;
using speedCode.Model;
using speedCode.Room.Model;


namespace speedCode.Excise;

using Microsoft.EntityFrameworkCore;

public class ExciseDb : DbContext
{
    public DbSet<TestCode> TestCode { get; set; }
    public DbSet<ExerciseTest> ExerciseTest { get; set; }
    public DbSet<Exercise> Exercise { get; set; }

    public DbSet<TestExpectIO> TestExpectIos { get; set; }

    public DbSet<RoomExercise> RoomExercise { get; set; }
    public DbSet<ProfileExercise> ProfileExercise { get; set; }

    public ExciseDb(DbContextOptions<ExciseDb> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Profiles>();
        modelBuilder.Ignore<Rooms>();

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

        modelBuilder.Entity<TestCode>()
            .Property(u => u.createdAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<TestCode>()
            .Property(p => p.Type)
            .HasConversion<string>();
    }
}