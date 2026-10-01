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
    public DbSet<ParkingTicket> ParkingTickets { get; set; }
    public DbSet<Payment> Payments { get; set; }
}