using BusBookingAPI.Models;

namespace BusBookingAPI.DTOs;

public record ApiResponse<T>(bool Success, T? Data, string Message);

public record OperatorSummaryDto(
    int Id,
    string Name,
    string Email,
    string? Phone,
    OperatorStatus Status,
    string HeadOfficeDistrict,
    int BusCount,
    decimal TotalRevenue
);

public record BusSummaryDto(
    int Id,
    string Name,
    int TotalSeats,
    BusStatus Status,
    DateTime CreatedAt
);

public record RouteDto(
    int Id,
    string Source,
    string Destination,
    DateTime CreatedAt
);

public record CreateRouteRequest(string Source, string Destination);

public record UpdateConvenienceFeeRequest(decimal Fee);
