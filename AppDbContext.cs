using LicenseApiProject.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseApiProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<License> Licenses => Set<License>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();

    modelBuilder.Entity<Device>()
        .HasIndex(d => d.DeviceIdentifier)
        .IsUnique();

    modelBuilder.Entity<License>()
        .HasOne(l => l.User)
        .WithMany(u => u.Licenses)
        .HasForeignKey(l => l.UserID)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<License>()
        .HasOne(l => l.Device)
        .WithMany(d => d.Licenses)
        .HasForeignKey(l => l.DeviceID)
        .OnDelete(DeleteBehavior.Cascade);
}
    }
}
