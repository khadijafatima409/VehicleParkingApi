using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParkingApi.Data;
using VehicleParkingApi.Services;

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
    public IActionResult GetAvailability(string? vehicleType)
    {
        var slotsQuery = _db.ParkingSlots.AsQueryable();

        if (!string.IsNullOrEmpty(vehicleType))
        {
            slotsQuery = slotsQuery.Where(s => s.VehicleType == vehicleType);
        }

        var slotsList = slotsQuery.ToList();
        return Ok(slotsList);
    }

    // Comes from the Singleton instead of hitting the database again -
    
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var tracker = ParkingSlotTracker.GetInstance();
        return Ok(new
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
        if (slot == null)
        {
            return NotFound("Slot not found");
        }
        if (slot.IsOccupied)
        {
            return BadRequest("Slot is already occupied");
        }

        slot.IsOccupied = true;
        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkOccupied();

        return Ok("Slot " + slot.SlotNumber + " is now occupied");
    }

    [HttpPut("{id}/release")]
    [Authorize(Roles = "Admin")]
    public IActionResult ReleaseSlot(int id)
    {
        var slot = _db.ParkingSlots.FirstOrDefault(s => s.Id == id);
        if (slot == null)
        {
            return NotFound("Slot not found");
        }
        if (!slot.IsOccupied)
        {
            return BadRequest("Slot is already free");
        }

        slot.IsOccupied = false;
        _db.SaveChanges();

        ParkingSlotTracker.GetInstance().MarkFreed();

        return Ok("Slot " + slot.SlotNumber + " is now free");
    }
}