using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<TestCode> TestCode { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<RoomUser> RoomUser { get; set; }
        public DbSet<RoomExercise> RoomExercise { get; set; }
        public DbSet<ExerciseTest> ExerciseTest { get; set; }
        public DbSet<ProfileExercise> ProfileExercise { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<ProfileAchivement> ProfileAchievement { get; set; }
        public DbSet<NavItem> NavItem { get; set; }
        public DbSet<DropdownItems> DropdownItems { get; set; }
        public DbSet<FooterSection> FooterSection { get; set; }
        public DbSet<FooterSectionLinks> FooterSectionLinks { get; set; }
        public DbSet<ContactInfo> ContactInfo { get; set; }
        public DbSet<SocialLink> SocialLink { get; set; }
        public DbSet<TestExpectIO> TestExpectIO { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profiles>(entity =>
            {
                entity.Property(p => p.role)
                    .HasConversion<string>()
                    .HasDefaultValue(speedCode.Model.Profiles.Rol.user);

                entity.Property(p => p.createdAt)
                    .HasDefaultValueSql("NOW()");
            });


            modelBuilder.Entity<Profiles>()
                .Property(p => p.role)
                .HasConversion<string>();


            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.Property(p => p.roomLevel)
                    .HasConversion<string>();

                entity.Property(p => p.createdAt)
                    .HasDefaultValueSql("NOW()");
            });


            modelBuilder.Entity<ExerciseTest>()
                .HasKey(et => new { et.exerciseId, et.testId });

            modelBuilder.Entity<ExerciseTest>()
                .HasOne(et => et.exercise)
                .WithMany(e => e.ExerciseTests)
                .HasForeignKey(et => et.exerciseId);

            modelBuilder.Entity<ExerciseTest>()
                .HasOne(et => et.test)
                .WithMany(t => t.ExerciseTests)
                .HasForeignKey(et => et.testId);

            modelBuilder.Entity<TestCode>()
                .Property(u => u.createdAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TestCode>()
                .Property(p => p.Type)
                .HasConversion<string>();
        }
    }
}