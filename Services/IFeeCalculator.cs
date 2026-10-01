using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public interface IFeeCalculator
{
    VehicleType SupportedType { get; }
    decimal CalculateFee(DateTime entryTime, DateTime exitTime);
}