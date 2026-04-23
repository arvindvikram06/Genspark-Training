using BusBookingAPI.Models;

namespace BusBookingAPI.DTOs;

public record CreateScheduleRequest(
    int BusId,
    int RouteId,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    decimal PricePerSeat
);

public record ScheduleResponse(
    int Id,
    int BusId,
    string BusName,
    int RouteId,
    string Source,
    string Destination,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    decimal PricePerSeat,
    string BoardingPoint,
    string DropPoint,
    ScheduleStatus Status
);
