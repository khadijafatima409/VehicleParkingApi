using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class CarFeeCalculator : HourlyFeeCalculator
{
    public override VehicleType SupportedType => VehicleType.Car;
    protected override decimal RatePerHour => 50m;
}