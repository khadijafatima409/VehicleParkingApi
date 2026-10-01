using Microsoft.EntityFrameworkCore;
using VehicleParkingApi.Data;
using VehicleParkingApi.Exceptions;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Services;

public class TicketService
{
    private readonly AppDbContext _db;
    private readonly FeeCalculatorResolver _feeCalculatorResolver;

    public TicketService(AppDbContext db, FeeCalculatorResolver feeCalculatorResolver)
    {
        _db = db;
        _feeCalculatorResolver = feeCalculatorResolver;
    }

    public ParkingTicket CreateTicket(string vehicleNumber, VehicleType vehicleType)
    {
        var freeSlot = _db.ParkingSlots.FirstOrDefault(s => s.VehicleType == vehicleType && !s.IsOccupied);
        if (freeSlot == null)
        {
            throw new SlotNotAvailableException("No free slot available for this vehicle type");
        }

        freeSlot.IsOccupied = true;

        var ticket = new ParkingTicket
        {
            VehicleNumber = vehicleNumber,
            VehicleType = vehicleType,
            ParkingSlotId = freeSlot.Id,
            EntryTime = DateTime.Now,
            IsClosed = false
        };

        _db.ParkingTickets.Add(ticket);
        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkOccupied();

        return ticket;
    }

    public ParkingTicket CloseTicket(int ticketId)
    {
        var ticket = _db.ParkingTickets
            .Include(t => t.ParkingSlot)
            .FirstOrDefault(t => t.Id == ticketId);

        if (ticket == null)
        {
            throw new TicketNotFoundException("Ticket not found");
        }
        if (ticket.IsClosed)
        {
            throw new TicketAlreadyClosedException("Ticket is already closed");
        }

        ticket.ExitTime = DateTime.Now;

        var calculator = _feeCalculatorResolver.GetCalculator(ticket.VehicleType);
        ticket.Fee = calculator.CalculateFee(ticket.EntryTime, ticket.ExitTime.Value);
        ticket.IsClosed = true;

        if (ticket.ParkingSlot != null)
        {
            ticket.ParkingSlot.IsOccupied = false;
        }

        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkFreed();

        return ticket;
    }
}