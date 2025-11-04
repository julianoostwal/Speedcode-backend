using Microsoft.EntityFrameworkCore;
using speedCode.exercise.model;
using speedCode.Model;
using speedCode.Profile.model;
using speedCode.Room.Model;

namespace speedCode.Room;

public class RoomsDb : DbContext
{
    public DbSet<Rooms> rooms { get; set; }
    public DbSet<RoomExercise> RoomExercise { get; set; }

    public DbSet<RoomUser> RoomUser { get; set; }

    public RoomsDb(DbContextOptions<RoomsDb> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestCode>().ToTable("testCode", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Exercise>().ToTable("exercises", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Profiles>().ToTable("profiles", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<CompletedProfileExercise>().ToTable("profile_exercises", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<TestExpectIO>().ToTable("testExpectIO", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<ExerciseTest>().ToTable("exercise_tests", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<Rooms>(entity =>
        {
            entity.Property(p => p.roomLevel)
                .HasConversion<string>();

            entity.Property(p => p.createdAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}