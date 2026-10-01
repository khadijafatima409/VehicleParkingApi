using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class FeeCalculatorResolver
{
    private readonly IEnumerable<IFeeCalculator> _calculators;

    public FeeCalculatorResolver(IEnumerable<IFeeCalculator> calculators)
    {
        _calculators = calculators;
    }

    public IFeeCalculator GetCalculator(VehicleType vehicleType)
    {
        var match = _calculators.FirstOrDefault(c => c.SupportedType == vehicleType);
        if (match == null)
        {
            throw new InvalidOperationException("No fee calculator found for " + vehicleType);
        }
        return match;
    }
}