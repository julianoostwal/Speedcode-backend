using Microsoft.EntityFrameworkCore;
using speedCode.Excise.model;
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
        modelBuilder.Ignore<Exercise>();
        modelBuilder.Ignore<TestCode>();
        modelBuilder.Ignore<TestExpectIO>();
        modelBuilder.Ignore<ExerciseTest>();
        modelBuilder.Ignore<Profiles>();
        
        modelBuilder.Entity<Rooms>(entity =>
        {
            entity.Property(p => p.roomLevel)
                .HasConversion<string>();

            entity.Property(p => p.createdAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}