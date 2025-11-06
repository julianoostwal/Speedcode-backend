using Microsoft.EntityFrameworkCore;
using speedCode.Exercise.model;
using speedCode.Profile.model;
using speedCode.Profile.Model;

namespace speedCode.Profile
{
    public class ProfileDb : DbContext
    {
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<ProfileAchivement> ProfileAchievement { get; set; }
        public DbSet<CompletedProfileExercise> CompletedProfileExercise { get; set; }
        public DbSet<CreatedProfileExercises> CreatedProfileExercise { get; set; }

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

            modelBuilder.Entity<CompletedProfileExercise>(entity =>
            {
                entity.ToTable("completed_profile_exercises");

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