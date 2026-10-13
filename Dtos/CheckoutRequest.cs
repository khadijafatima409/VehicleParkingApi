using VehicleParkingApi.Models;

namespace VehicleParkingApi.Dtos;

public class CheckoutRequest
{
    public PaymentMethod Method { get; set; }
    public decimal AmountGiven { get; set; }
}