using Microsoft.EntityFrameworkCore;
using speedCode.Model;
using speedCode.Profile.model;

namespace speedCode.Profile
{
    public class ProfileDb : DbContext
    {
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<ProfileAchivement> ProfileAchievement { get; set; }
        public DbSet<RoomUser> RoomUser { get; set; }

        public ProfileDb(DbContextOptions<ProfileDb> options)
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


            modelBuilder.Entity<Profiles>()
                .Property(p => p.role)
                .HasConversion<string>();
        }
    }
}