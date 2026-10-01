namespace VehicleParkingApi.Models;

public class ParkingTicket
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public int ParkingSlotId { get; set; }
    public ParkingSlot? ParkingSlot { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public decimal? Fee { get; set; }
    public bool IsClosed { get; set; }
}