using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class CarFeeCalculator : IFeeCalculator
{
    public VehicleType SupportedType => VehicleType.Car;

    public decimal CalculateFee(DateTime entryTime, DateTime exitTime)
    {
        var hours = Math.Ceiling((exitTime - entryTime).TotalHours);
        if (hours < 1)
        {
            hours = 1;
        }
        return (decimal)hours * 2.0m;
    }
}