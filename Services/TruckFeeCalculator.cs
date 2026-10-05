using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class TruckFeeCalculator : HourlyFeeCalculator
{
    public override VehicleType SupportedType => VehicleType.Truck;
    protected override decimal RatePerHour => 100m;
}