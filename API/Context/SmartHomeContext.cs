namespace API.Context;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class SmartHomeContext : DbContext
{
    public DbSet<Device> Devices { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomDevice> RoomDevices { get; set; }
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
}
