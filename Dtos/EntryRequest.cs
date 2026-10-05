using System.ComponentModel.DataAnnotations;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Dtos;

public class EntryRequest
{
    [Required, StringLength(20)]
    public string VehicleNumber { get; set; } = string.Empty;

    public VehicleType VehicleType { get; set; }
}