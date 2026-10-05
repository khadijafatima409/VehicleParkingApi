using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class BikeFeeCalculator : HourlyFeeCalculator
{
    public override VehicleType SupportedType => VehicleType.Bike;
    protected override decimal RatePerHour => 20m;
}