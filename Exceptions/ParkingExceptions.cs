namespace VehicleParkingApi.Exceptions;

public class ParkingException : Exception
{
    public ParkingException(string message) : base(message) { }
}

public class SlotNotAvailableException : ParkingException
{
    public SlotNotAvailableException(string message) : base(message) { }
}

public class TicketNotFoundException : ParkingException
{
    public TicketNotFoundException(string message) : base(message) { }
}

public class TicketAlreadyClosedException : ParkingException
{
    public TicketAlreadyClosedException(string message) : base(message) { }
}

public class TicketNotClosedException : ParkingException
{
    public TicketNotClosedException(string message) : base(message) { }
}

public class PaymentAlreadyMadeException : ParkingException
{
    public PaymentAlreadyMadeException(string message) : base(message) { }
}

public class VehicleAlreadyParkedException : ParkingException
{
    public VehicleAlreadyParkedException(string message) : base(message) { }
}