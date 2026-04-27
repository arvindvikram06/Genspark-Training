using BusBookingAPI.Models;
using System.Text.Json;

namespace BusBookingAPI.DTOs;

public record OfficeSubmissionRequest(
    string District,
    string Address
);

public record OfficeResponse(
    int Id,
    string District,
    string Address
);

public record CreateBusRequest(
    string Name,
    int TotalSeats,
    JsonElement SeatLayout
);

public record BusResponse(
    int Id,
    string Name,
    int TotalSeats,
    JsonElement? SeatLayout,
    BusStatus Status,
    DateTime CreatedAt,
    bool IsDeleted,
    DateTime? DisabledFrom,
    DateTime? DisabledTo
);

public record DisableBusRequest(
    int BusId,
    DateTime DisabledFrom,
    DateTime DisabledTo
);

public record BookingSummaryDto(
    int BookingId,
    string UserName,
    string UserEmail,
    DateTime DepartureTime,
    decimal TotalAmount,
    BookingStatus Status,
    IEnumerable<string> SeatNumbers
);
