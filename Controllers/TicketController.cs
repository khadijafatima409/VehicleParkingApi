using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Dtos;
using VehicleParkingApi.Services;

namespace VehicleParkingApi.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly PaymentService _paymentService;

    public TicketController(TicketService ticketService, PaymentService paymentService)
    {
        _ticketService = ticketService;
        _paymentService = paymentService;
    }

    [HttpPost("entry")]
    public IActionResult Entry(EntryRequest request)
    {
        
        var ticket = _ticketService.CreateTicket(request.VehicleNumber, request.VehicleType);
        return Ok(ticket);
        
    }

    [HttpPut("{id}/exit")]
    public IActionResult Exit(int id)
    {
        var ticket = _ticketService.CloseTicket(id);
        return Ok(ticket);
        
    }

    [HttpPost("{id}/payment")]
    public IActionResult Pay(int id, PaymentRequest request)
    {
    
        var payment = _paymentService.RecordPayment(id, request.Method);
        var response = new PaymentResponse
        {
            Id = payment.Id,
            ParkingTicketId = payment.ParkingTicketId,
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            PaidAt = payment.PaidAt
        };
        return Ok(response);
    }
}