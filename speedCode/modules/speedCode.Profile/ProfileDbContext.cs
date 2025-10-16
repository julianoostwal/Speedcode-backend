using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Profile;

public class ProfileDbContext : DbContext
{
    public DbSet<Profiles> Profiles { get; set; }

    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profiles>(entity =>
        {
            entity.Property(p => p.role)
                .HasConversion<int>()
                .HasDefaultValue(Rol.user);

            entity.Property(p => p.createdAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}