using Microsoft.EntityFrameworkCore;
using speedCode.Room.Model;

namespace speedCode.Room;

public class RoomsDb : DbContext
{
    public DbSet<Rooms> rooms { get; set; }

    public RoomsDb(DbContextOptions<RoomsDb> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rooms>(entity =>
        {
            entity.Property(p => p.roomLevel)
                .HasConversion<string>();

            entity.Property(p => p.createdAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}