namespace VehicleParkingApi.Dtos;

public class PaymentResponse
{
    public int Id { get; set; }
    public int ParkingTicketId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
}