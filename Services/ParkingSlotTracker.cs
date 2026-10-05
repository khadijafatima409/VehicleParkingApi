using VehicleParkingApi.Data;

namespace VehicleParkingApi.Services;

// Classic Singleton: only one object of this class can exist in the whole app.
// sealed  = nobody can inherit from it
// private constructor = nobody outside can write "new ParkingSlotTracker()"
public sealed class ParkingSlotTracker
{
    private static ParkingSlotTracker? _instance;
    private static readonly object _lockObj = new object();

    public int TotalSlots { get; private set; }
    public int OccupiedSlots { get; private set; }

    private ParkingSlotTracker()
    {
        // Reflection can call a private constructor, so this stops a second object
        if (_instance != null)
        {
            throw new InvalidOperationException(
                "An instance of ParkingSlotTracker already exists. Use GetInstance() instead.");
        }
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

    public int GetAvailableCount()
    {
        lock (_lockObj)
        {
            return TotalSlots - OccupiedSlots;
        }
    }
}