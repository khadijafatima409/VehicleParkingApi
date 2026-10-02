using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Dtos;
using VehicleParkingApi.Exceptions;
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
        try
        {
            var ticket = _ticketService.CreateTicket(request.VehicleNumber, request.VehicleType);
            return Ok(ticket);
        }
        catch (ParkingException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/exit")]
    public IActionResult Exit(int id)
    {
        try
        {
            var ticket = _ticketService.CloseTicket(id);
            return Ok(ticket);
        }
        catch (TicketNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ParkingException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/payment")]
    public IActionResult Pay(int id, PaymentRequest request)
    {
        try
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
        catch (TicketNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ParkingException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}