using VehicleParkingApi.Data;
using VehicleParkingApi.Exceptions;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class CheckoutService
{
    private readonly AppDbContext _db;
    private readonly TicketService _ticketService;
    private readonly PaymentService _paymentService;

    public CheckoutService(AppDbContext db, TicketService ticketService, PaymentService paymentService)
    {
        _db = db;
        _ticketService = ticketService;
        _paymentService = paymentService;
    }

    public Payment Checkout(int ticketId, PaymentMethod method, decimal amountGiven)
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {
            // Step 1: close the ticket, calculate the fee, free the slot (SaveChanges #1)
            var ticket = _ticketService.CloseTicket(ticketId);

            // Step 2: a rule that can fail AFTER step 1 is already saved
            if (amountGiven < ticket.Fee)
            {
                throw new InsufficientPaymentException(
                    "Amount given (Rs " + amountGiven + ") is less than the fee (Rs " + ticket.Fee + ")");
            }

            // Step 3: record the payment (SaveChanges #2)
            var payment = _paymentService.RecordPayment(ticketId, method);

            transaction.Commit();
            return payment;
        }
        catch
        {
            transaction.Rollback();
            ParkingSlotTracker.GetInstance().LoadFromDb(_db);
            throw;
        }
    }
}