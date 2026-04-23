namespace BusBookingAPI.Models;

public enum UserRole
{
    User,
    Operator,
    Admin
}

public enum OperatorStatus
{
    Pending,
    Approved,
    Disabled
}

public enum BusStatus
{
    Pending,
    Approved,
    Disabled
}

public enum ScheduleStatus
{
    Pending,
    Approved,
    Disabled
}

public enum SeatStatus
{
    Available,
    Held,
    Booked
}

public enum BookingStatus
{
    Confirmed,
    Cancelled,
    Pending
}
