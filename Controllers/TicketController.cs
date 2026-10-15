using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Dtos;
using VehicleParkingApi.Services;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly PaymentService _paymentService;
    private readonly CheckoutService _checkoutService;

    public TicketController(TicketService ticketService, PaymentService paymentService, CheckoutService checkoutService)
    {
        _ticketService = ticketService;
        _paymentService = paymentService;
        _checkoutService = checkoutService;
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
        return Ok(ToPaymentResponse(payment));
    }

    [HttpPost("{id}/checkout")]
    public IActionResult Checkout(int id, CheckoutRequest request)
    {
        var payment = _checkoutService.Checkout(id, request.Method, request.AmountGiven);
        return Ok(ToPaymentResponse(payment));
    }

    private static PaymentResponse ToPaymentResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            ParkingTicketId = payment.ParkingTicketId,
            Amount = payment.Amount,
            Method = payment.Method.ToString(),
            PaidAt = payment.PaidAt
        };
    }
}