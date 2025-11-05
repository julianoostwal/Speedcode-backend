using speedCode.Exercise.model;


namespace speedCode.exercise;

using Microsoft.EntityFrameworkCore;

public class ExerciseDb : DbContext
{
    public DbSet<TestCode> TestCode { get; set; }
    public DbSet<ExerciseTest> ExerciseTest { get; set; }
    public DbSet<Exercise.model.Exercise> Exercise { get; set; }

    public DbSet<TestExpectIO> TestExpectIos { get; set; }


    public ExerciseDb(DbContextOptions<ExerciseDb> options)
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

        modelBuilder.Entity<TestCode>()
            .Property(u => u.createdAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<TestCode>()
            .Property(p => p.Type)
            .HasConversion<string>();
    }
}