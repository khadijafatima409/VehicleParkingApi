using VehicleParkingApi.Models;

namespace VehicleParkingApi.Dtos;

public class PaymentRequest
{
    public PaymentMethod Method { get; set; }
}