namespace VehicleParkingApi.Models;

public class Payment
{
    public int Id { get; set; }
    public int ParkingTicketId { get; set; }
    public ParkingTicket? ParkingTicket { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; }
}