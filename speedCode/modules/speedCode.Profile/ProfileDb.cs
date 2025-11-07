using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<Profiles>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(p => p.id);

                entity.Property(p => p.role)
                    .HasConversion<string>()
                    .HasDefaultValue(Rol.user)
                    .IsRequired();

                entity.Property(p => p.createdAt)
                    .HasDefaultValueSql("NOW()")
                    .IsRequired();

                entity.Property(p => p.nickName)
                    .IsRequired();

                entity.Property(p => p.avatarUrl)
                    .IsRequired(false);

                entity.HasIndex(p => p.id)
                    .IsUnique();
            });

            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.ToTable("achievements");
                entity.HasKey(a => a.id);

                entity.Property(a => a.achievementName)
                    .IsRequired();

                entity.Property(a => a.achievementDescription)
                    .IsRequired();

                entity.Property(a => a.achievementIcon)
                    .IsRequired();
            });

            modelBuilder.Entity<CompletedProfileExercise>(entity =>
            {
                entity.ToTable("completed_profile_exercises");
                entity.HasKey(pe => pe.id);

                entity.Property(pe => pe.profileId)
                    .IsRequired();

                entity.Property(pe => pe.exerciseId)
                    .IsRequired();

                entity.Property(pe => pe.time)
                    .IsRequired();

                entity.Property(pe => pe.memory)
                    .IsRequired();

                entity.HasIndex(pe => new { pe.profileId, pe.exerciseId });

                entity.HasOne<Profiles>()
                    .WithMany()
                    .HasForeignKey(pe => pe.profileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CreatedProfileExercises>(entity =>
            {
                entity.ToTable("created_profile_exercises");
                entity.HasKey(cpe => cpe.id);

                entity.Property(cpe => cpe.profileId)
                    .IsRequired();

                entity.Property(cpe => cpe.exerciseId)
                    .IsRequired();

                entity.HasIndex(cpe => new { cpe.profileId, cpe.exerciseId })
                    .IsUnique();

                entity.HasOne<Profiles>()
                    .WithMany()
                    .HasForeignKey(cpe => cpe.profileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProfileAchivement>(entity =>
            {
                entity.ToTable("profile_achievements");
                entity.HasKey(pa => new { pa.profileId, pa.achievementId });

                entity.Property(pa => pa.profileId)
                    .IsRequired();

                entity.Property(pa => pa.achievementId)
                    .IsRequired();

                entity.HasOne<Profiles>()
                    .WithMany()
                    .HasForeignKey(pa => pa.profileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Achievement>()
                    .WithMany()
                    .HasForeignKey(pa => pa.achievementId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}