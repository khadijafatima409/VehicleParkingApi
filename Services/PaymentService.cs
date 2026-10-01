using VehicleParkingApi.Data;
using VehicleParkingApi.Exceptions;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class PaymentService
{
    private readonly AppDbContext _db;

    public PaymentService(AppDbContext db)
    {
        _db = db;
    }

    public Payment RecordPayment(int ticketId, PaymentMethod method)
    {
        var ticket = _db.ParkingTickets.FirstOrDefault(t => t.Id == ticketId);
        if (ticket == null)
        {
            throw new TicketNotFoundException("Ticket not found");
        }
        if (!ticket.IsClosed || ticket.Fee == null)
        {
            throw new TicketNotClosedException("Ticket must be closed with a fee before payment");
        }

        var alreadyPaid = _db.Payments.Any(p => p.ParkingTicketId == ticketId);
        if (alreadyPaid)
        {
            throw new PaymentAlreadyMadeException("This ticket has already been paid");
        }

        var payment = new Payment
        {
            ParkingTicketId = ticketId,
            Amount = ticket.Fee.Value,
            Method = method,
            PaidAt = DateTime.Now
        };

        _db.Payments.Add(payment);
        _db.SaveChanges();

        return payment;
    }
}