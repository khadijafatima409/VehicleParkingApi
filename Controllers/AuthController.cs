using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Data;
using VehicleParkingApi.Dtos;
using VehicleParkingApi.Models;
using VehicleParkingApi.Services;

namespace VehicleParkingApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var existingUser = _db.Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUser != null)
        {
            return BadRequest("This username is already taken");
        }

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Username or password is wrong");
        }

        var token = _tokenService.CreateToken(user);
        return Ok(new { token = token });
    }
}