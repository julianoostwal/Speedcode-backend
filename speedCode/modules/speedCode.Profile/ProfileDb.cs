using Microsoft.EntityFrameworkCore;
using speedCode.Excise.model;
using speedCode.Model;
using speedCode.Profile.model;

namespace speedCode.Profile
{
    public class ProfileDb : DbContext
    {
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<ProfileAchivement> ProfileAchievement { get; set; }
        public DbSet<ProfileExercise> ProfileExercise { get; set; }

        public ProfileDb(DbContextOptions<ProfileDb> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestCode>(entity =>
            {
                entity.Property(p => p.Type)
                    .HasConversion<string>();

                entity.Property(p => p.createdAt)
                    .HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<ProfileExercise>(entity =>
            {
                entity.ToTable("profile_exercises");

                entity.HasKey(pe => pe.id);

                entity.HasOne(pe => pe.profile)
                    .WithMany(p => p.ProfileExercises)
                    .HasForeignKey(pe => pe.profileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pe => pe.exercise)
                    .WithMany()
                    .HasForeignKey(pe => pe.exerciseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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

            modelBuilder.Entity<ProfileAchivement>()
                .HasKey(pa => new { pa.profileId, pa.achievementId });
        }
    }
}