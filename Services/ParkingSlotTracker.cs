using VehicleParkingApi.Data;

namespace VehicleParkingApi.Services;

// Classic Singleton pattern, written by hand so you can see the
// mechanics: a private constructor stops anyone else from doing
// "new ParkingSlotTracker()", and one static field holds the only
// copy that will ever exist. Every controller that asks for it
// gets the SAME object back, so the slot counts stay consistent
// instead of every request having its own separate copy.
public class ParkingSlotTracker
{
    private static ParkingSlotTracker? _instance;
    private static readonly object _lockObj = new object();

    public int TotalSlots { get; private set; }
    public int OccupiedSlots { get; private set; }

    private ParkingSlotTracker()
    {
    }

    public static ParkingSlotTracker GetInstance()
    {
        if (_instance == null)
        {
            lock (_lockObj)
            {
                if (_instance == null)
                {
                    _instance = new ParkingSlotTracker();
                }
            }
        }
        return _instance;
    }

    public void LoadFromDb(AppDbContext db)
    {
        lock (_lockObj)
        {
            TotalSlots = db.ParkingSlots.Count();
            OccupiedSlots = db.ParkingSlots.Count(s => s.IsOccupied);
        }
    }

    public int GetAvailableCount()
    {
        return TotalSlots - OccupiedSlots;
    }

    public void MarkOccupied()
    {
        lock (_lockObj)
        {
            if (OccupiedSlots < TotalSlots)
            {
                OccupiedSlots++;
            }
        }
    }

    public void MarkFreed()
    {
        lock (_lockObj)
        {
            if (OccupiedSlots > 0)
            {
                OccupiedSlots--;
            }
        }
    }
}