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
    Disabled,
    Cancelled,
    Ongoing,
    Completed
}

public enum SeatStatus
{
    Available,
    Held,
    Booked
}

public enum BookingStatus
{
    PendingPayment,
    Confirmed,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Success,
    Failed,
    Refunded,
    Timeout
}
