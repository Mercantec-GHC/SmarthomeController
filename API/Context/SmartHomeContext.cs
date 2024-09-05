namespace API.Context;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public class SmartHomeContext : DbContext
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomDevice> RoomDevices { get; set; }
    public DbSet<SoundUnit> SoundUnits { get; set; }
    public DbSet<Sound> Sounds { get; set; }
    public SmartHomeContext(DbContextOptions<SmartHomeContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the composite key in the many-to-many relationship
        modelBuilder.Entity<RoomDevice>()
            .HasKey(rd => rd.RoomDeviceId);

        modelBuilder.Entity<RoomDevice>()
            .HasOne(rd => rd.Room)
            .WithMany(r => r.RoomDevices)
            .HasForeignKey(rd => rd.RoomId);

        modelBuilder.Entity<RoomDevice>()
            .HasOne(rd => rd.Device)
            .WithMany(d => d.RoomDevices)
            .HasForeignKey(rd => rd.DeviceId);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Common && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((Common)entityEntry.Entity).UpdatedAt = DateTime.UtcNow.AddHours(2);

            if (entityEntry.State == EntityState.Added)
            {
                ((Common)entityEntry.Entity).CreatedAt = DateTime.UtcNow.AddHours(2);
            }
        }

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Common && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((Common)entityEntry.Entity).UpdatedAt = DateTime.UtcNow.AddHours(2);

            if (entityEntry.State == EntityState.Added)
            {
                ((Common)entityEntry.Entity).CreatedAt = DateTime.UtcNow.AddHours(2);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

