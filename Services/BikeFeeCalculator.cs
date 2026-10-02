using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class BikeFeeCalculator : IFeeCalculator
{
    public VehicleType SupportedType => VehicleType.Bike;

    public decimal CalculateFee(DateTime entryTime, DateTime exitTime)
    {
        var hours = Math.Ceiling((exitTime - entryTime).TotalHours);
        if (hours < 1)
        {
            hours = 1;
        }
        return (decimal)hours * 20.0m;
    }
}