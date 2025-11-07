using Microsoft.EntityFrameworkCore;
using speedCode.Exercise.model;


namespace speedCode.exercise;

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
        modelBuilder.Entity<Exercise.model.Exercise>(entity =>
        {
            entity.ToTable("exercises");
            entity.HasKey(e => e.id);

            entity.Property(e => e.title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.description)
                .IsRequired();

            entity.Property(e => e.createdAt)
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            entity.Property(e => e.isRoomExercise)
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(e => e.isPrivate)
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(e => e.points)
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(e => e.Level)
                .HasConversion<string>()
                .IsRequired();

            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.isPrivate);
        });

        modelBuilder.Entity<TestCode>(entity =>
        {
            entity.ToTable("testCode");
            entity.HasKey(tc => tc.id);

            entity.Property(tc => tc.Type)
                .HasConversion<string>()
                .HasColumnName("type")
                .IsRequired();

            entity.Property(tc => tc.LanguageId)
                .HasColumnName("languageId")
                .IsRequired();

            entity.Property(tc => tc.ACRegex)
                .HasColumnName("AC_regex");

            entity.Property(tc => tc.createdAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        });

        modelBuilder.Entity<TestExpectIO>(entity =>
        {
            entity.ToTable("testExpectIO");
            entity.HasKey(te => te.id);

            entity.Property(te => te.testCodeId)
                .IsRequired();

            entity.Property(te => te.input);

            entity.Property(te => te.output)
                .IsRequired();

            entity.HasIndex(te => te.testCodeId);
            
            entity.HasOne<TestCode>()
                .WithMany()
                .HasForeignKey(te => te.testCodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExerciseTest>(entity =>
        {
            entity.ToTable("exercise_tests");
            entity.HasKey(et => et.id);

            entity.Property(et => et.id)
                .IsRequired();

            entity.Property(et => et.exerciseId)
                .IsRequired();

            entity.Property(et => et.testId)
                .IsRequired();

            entity.HasIndex(et => new { et.exerciseId, et.testId })
                .IsUnique();
            
            entity.HasIndex(et => et.exerciseId);
            entity.HasIndex(et => et.testId);
            
            entity.HasOne<Exercise.model.Exercise>()
                .WithMany()
                .HasForeignKey(et => et.exerciseId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne<TestCode>()
                .WithMany()
                .HasForeignKey(et => et.testId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}