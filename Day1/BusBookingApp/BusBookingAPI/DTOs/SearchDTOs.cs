using BusBookingAPI.Models;
using System.Text.Json;

namespace BusBookingAPI.DTOs;

public record BusSearchResponse(
    int ScheduleId,
    string BusName,
    string OperatorName,
    string Source,
    string Destination,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    decimal PricePerSeat,
    int TotalSeats,
    int AvailableSeats,
    string BoardingPoint,
    string DropPoint
);

public record ScheduleSeatMapResponse(
    int ScheduleId,
    JsonElement? SeatLayout,
    IEnumerable<SeatStatusResponse> Seats
);

public record SeatStatusResponse(
    string SeatNumber,
    SeatStatus Status
);

public record ScheduleBookingSummary(
    int ScheduleId,
    int TotalBookings,
    int TotalPassengers,
    decimal TotalRevenue,
    IEnumerable<BookingDetailDto> Bookings
);

public record BookingDetailDto(
    int BookingId,
    string UserName,
    DateTime CreatedAt,
    BookingStatus Status,
    IEnumerable<string> SeatNumbers
);
