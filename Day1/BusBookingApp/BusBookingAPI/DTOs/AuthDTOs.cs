namespace BusBookingAPI.DTOs;

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string? Phone
);

public record OperatorRegisterRequest(
    string Name,
    string Email,
    string Password,
    string? Phone,
    string HeadOfficeDistrict
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string Token,
    int UserId,
    string Email,
    string Role
);
