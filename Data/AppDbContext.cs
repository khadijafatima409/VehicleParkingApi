using Microsoft.EntityFrameworkCore;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ParkingSlot> ParkingSlots { get; set; }
}