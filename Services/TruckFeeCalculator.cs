using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class TruckFeeCalculator : IFeeCalculator
{
    public VehicleType SupportedType => VehicleType.Truck;

    public decimal CalculateFee(DateTime entryTime, DateTime exitTime)
    {
        var hours = Math.Ceiling((exitTime - entryTime).TotalHours);
        if (hours < 1)
        {
            hours = 1;
        }
        return (decimal)hours * 4.0m;
    }
}