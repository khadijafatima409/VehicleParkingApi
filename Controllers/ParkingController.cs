using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Data;
using VehicleParkingApi.Services;
using VehicleParkingApi.Models;

namespace VehicleParkingApi.Controllers;

[ApiController]
[Route("api/parkingslots")]
[Authorize]
public class ParkingController : ControllerBase
{
    private readonly AppDbContext _db;

    public ParkingController(AppDbContext db) 
    {
        _db = db;
    }

    [HttpGet("availability")]
    public IActionResult GetAvailability(VehicleType? vehicleType, bool onlyFree = true)
    {
        var slotsQuery = _db.ParkingSlots.AsQueryable();

        if (vehicleType != null)
        {
            slotsQuery = slotsQuery.Where(s => s.VehicleType == vehicleType);
        }

        if (onlyFree)
        {
            slotsQuery = slotsQuery.Where(s => !s.IsOccupied);
        }

        return Ok(slotsQuery.ToList());
    }

    // Comes from the Singleton instead of hitting the database again -
    
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var tracker = ParkingSlotTracker.GetInstance(); // am getting singleton
        return Ok(new //return summary -  anonymous object 
        {
            totalSlots = tracker.TotalSlots,
            availableSlots = tracker.GetAvailableCount()
        });
    }


    [HttpPut("{id}/occupy")]
    [Authorize(Roles = "Admin")]
    public IActionResult OccupySlot(int id)
    {
        var slot = _db.ParkingSlots.FirstOrDefault(s => s.Id == id);
        if (slot == null) // 404 if not exist
        {
            return NotFound("Slot not found");
        }
        if (slot.IsOccupied) // if already occupied 400
        {
            return BadRequest("Slot is already occupied");
        }

        slot.IsOccupied = true;
        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkOccupied();

        return Ok("Slot " + slot.SlotNumber + " is now occupied"); //  suucces 200 ok
    }

    [HttpPut("{id}/release")]
    [Authorize(Roles = "Admin")]
    public IActionResult ReleaseSlot(int id)
    {
        var slot = _db.ParkingSlots.FirstOrDefault(s => s.Id == id);
        if (slot == null)
        {
            return NotFound("Slot not found");  // 404 not found 
        }
        if (!slot.IsOccupied)
        {
            return BadRequest("Slot is already free"); //400
        }

        slot.IsOccupied = false; // occoupied to available
        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkFreed();

        return Ok("Slot " + slot.SlotNumber + " is now free");
    }
}