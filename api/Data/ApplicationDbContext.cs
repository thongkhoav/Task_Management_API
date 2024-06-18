using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<Models.RefreshToken> RefreshTokens { get; set; }
        public DbSet<Models.Room> Rooms { get; set; }
        public DbSet<Models.ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Models.TaskModel> Tasks { get; set; }
        public DbSet<Models.UserRoom> UserRooms { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseNpgsql("Host=localhost; Database=task-manage-db; Username=postgres; Password=123456");
        // }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
            .Property(r => r.InviteCode)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ApplicationUser>(
                u =>
                {
                    // unique
                    u.Property(x => x.Email).IsRequired();
                }
            );

            modelBuilder.Entity<Models.UserRoom>()
                .HasKey(x => new { x.UserId, x.RoomId });

            // User many to many Room
            modelBuilder.Entity<Models.UserRoom>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRooms)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Models.UserRoom>()
                .HasOne(x => x.Room)
                .WithMany(x => x.UserRooms)
                .HasForeignKey(x => x.RoomId);

            // User one to many Task
            modelBuilder.Entity<Models.TaskModel>()
                .HasOne(x => x.User)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.UserId);

            // Room one to many Task
            modelBuilder.Entity<Models.TaskModel>()
                .HasOne(x => x.Room)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.RoomId);
        }

    }
}