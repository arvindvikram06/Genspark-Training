using BusBookingAPI.Models;

namespace BusBookingAPI.DTOs;

public record SeatHoldRequest(
    int ScheduleId,
    string[] SeatNumbers
);

public record SeatHoldResponse(
    int HoldId,
    DateTime ExpiresAt
);

public record PaymentInitiationResponse(
    int PaymentId,
    decimal Amount,
    string Message
);

public record BookingConfirmRequest(
    int HoldId,
    IEnumerable<PassengerDto> Passengers
);

public record PassengerDto(
    string SeatNumber,
    string Name,
    int Age,
    string Gender
);

public record BookingResponse(
    int Id,
    int ScheduleId,
    string BusName,
    string Source,
    string Destination,
    DateTime DepartureTime,
    decimal TotalAmount,
    BookingStatus Status,
    IEnumerable<PassengerDto> Passengers,
    string BoardingPoint,
    string DropPoint
);

public record BookingSummaryResponse(
    int Id,
    string Source,
    string Destination,
    DateTime DepartureTime,
    decimal TotalAmount,
    BookingStatus Status
);

public record ActiveSeatHoldResponse(
    int HoldId,
    string[] SeatNumbers,
    DateTime ExpiresAt
);

public record PaymentStatusResponse(
    int PaymentId,
    int BookingId,
    PaymentStatus Status,
    decimal Amount,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    string? FailureReason,
    int RetryCount
);
