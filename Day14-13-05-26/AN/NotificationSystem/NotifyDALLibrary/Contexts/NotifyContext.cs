

using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

using NotifyModelLibrary;

namespace NotifyDALLibrary.Contexts
{
    public class NotifyContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=NotifyApp;Username=postgres;Password=Arvind");

        }

        public DbSet<User> Users {get; set;}

        public DbSet<Notification> Notifications {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(u => u.Id).HasName("PK_UserID");
                u.HasIndex(u => u.Email).IsUnique();
                u.HasIndex(u => u.PhoneNumber).IsUnique();
                u.HasData(new User(1001,"arvind","arvind@gmail.com","9342394849"));
                u.HasQueryFilter(u => !u.IsDeleted); // Global Query Filter for Soft Delete
            });

            modelBuilder.Entity<Notification>(n =>
            {
                n.HasKey(n => n.Id).HasName("Pk_NotificationID");
                n.HasOne(n => n.user).WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .HasConstraintName("FK_Notification_User")
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}