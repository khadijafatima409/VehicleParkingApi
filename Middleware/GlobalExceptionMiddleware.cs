using VehicleParkingApi.Dtos;
using VehicleParkingApi.Exceptions;

namespace VehicleParkingApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next; // the next step in the pipeline; calling it lets the request continue
    private readonly ILogger<GlobalExceptionMiddleware> _logger; // _logger record errors 

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        int statusCode = ex switch
        {
            TicketNotFoundException => StatusCodes.Status404NotFound,
            ParkingException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        string message;
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(ex, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
            message = "Something went wrong. Please try again later.";
        }
        else
        {
            message = ex.Message;
        }

        var error = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }
}
