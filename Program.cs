using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehicleParkingApi.Data;
using VehicleParkingApi.Models;
using VehicleParkingApi.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only your token here"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Singleton lifetime = ASP.NET Core's built-in way of giving you
// exactly one shared instance for the app's lifetime
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<IFeeCalculator, CarFeeCalculator>();
builder.Services.AddScoped<IFeeCalculator, BikeFeeCalculator>();
builder.Services.AddScoped<IFeeCalculator, TruckFeeCalculator>();
builder.Services.AddScoped<FeeCalculatorResolver>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PaymentService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
//
var app = builder.Build();
   app.UseDefaultFiles();
   app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.ParkingSlots.Any())
    {
        db.ParkingSlots.AddRange(
            new ParkingSlot { SlotNumber = "A1", VehicleType = VehicleType.Car, IsOccupied = false },
            new ParkingSlot { SlotNumber = "A2", VehicleType = VehicleType.Car, IsOccupied = true },
            new ParkingSlot { SlotNumber = "A3", VehicleType = VehicleType.Car, IsOccupied = false },
            new ParkingSlot { SlotNumber = "B1", VehicleType = VehicleType.Bike, IsOccupied = false },
            new ParkingSlot { SlotNumber = "B2", VehicleType = VehicleType.Bike, IsOccupied = true },
            new ParkingSlot { SlotNumber = "T1", VehicleType = VehicleType.Truck, IsOccupied = false }
        );
        db.SaveChanges();
    }

    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        });
        db.SaveChanges();
    }

    ParkingSlotTracker.GetInstance().LoadFromDb(db);
}

app.Run();