using Microsoft.EntityFrameworkCore;
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
        modelBuilder.Entity<Rooms>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(r => r.id);
            
            entity.Property(r => r.roomName)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(r => r.roomLevel)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(r => r.createdAt)
                .HasDefaultValueSql("NOW()")
                .IsRequired();
                
            entity.Property(r => r.isPrivate)
                .HasDefaultValue(false)
                .IsRequired();
                
            entity.Property(r => r.CreatedById)
                .IsRequired();
                
            entity.HasIndex(r => r.CreatedById);
        });

        modelBuilder.Entity<RoomUser>(entity =>
        {
            entity.ToTable("room_users");
            entity.HasKey(ru => ru.id);
            
            entity.Property(ru => ru.roomId)
                .IsRequired();
                
            entity.Property(ru => ru.profileId)
                .IsRequired();
                
            entity.Property(ru => ru.createdAt)
                .HasDefaultValueSql("NOW()")
                .IsRequired();
            
            entity.HasIndex(ru => new { ru.roomId, ru.profileId })
                .IsUnique();
                
            entity.HasIndex(ru => ru.roomId);
            entity.HasIndex(ru => ru.profileId);
            
            entity.HasOne<Rooms>()
                .WithMany()
                .HasForeignKey(ru => ru.roomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoomExercise>(entity =>
        {
            entity.ToTable("room_exercises");
            entity.HasKey(re => re.id);
            
            entity.Property(re => re.roomId)
                .IsRequired();
                
            entity.Property(re => re.exerciseId)
                .IsRequired();
            
            entity.HasIndex(re => new { re.roomId, re.exerciseId })
                .IsUnique();
                
            entity.HasIndex(re => re.roomId);
            entity.HasIndex(re => re.exerciseId);
            
            entity.HasOne<Rooms>()
                .WithMany()
                .HasForeignKey(re => re.roomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}