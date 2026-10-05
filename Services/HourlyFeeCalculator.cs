using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

// Shared rounding rule for every vehicle type.
// Each child class only says which type it handles and its hourly rate.
public abstract class HourlyFeeCalculator : IFeeCalculator
{
    public abstract VehicleType SupportedType { get; }
    protected abstract decimal RatePerHour { get; }

    public decimal CalculateFee(DateTime entryTime, DateTime exitTime)
    {
        var hours = Math.Ceiling((exitTime - entryTime).TotalHours);
        if (hours < 1)
        {
            hours = 1;
        }
        return (decimal)hours * RatePerHour;
    }
}