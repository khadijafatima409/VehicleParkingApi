using VehicleParkingApi.Models;

namespace VehicleParkingApi.Dtos;

public class EntryRequest
{
    public string VehicleNumber { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
}