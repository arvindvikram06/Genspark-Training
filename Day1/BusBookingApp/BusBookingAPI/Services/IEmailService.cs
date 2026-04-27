namespace BusBookingAPI.Services;

public interface IEmailService
{
    Task SendBookingConfirmationEmailAsync(string toEmail, string userName, int bookingId, string busName, string route, string departureTime, decimal amount);
    Task SendPaymentFailureEmailAsync(string toEmail, string userName, int bookingId, string failureReason);
    Task SendCancellationEmailAsync(string toEmail, string userName, string source, string destination, DateTime departureTime, decimal amount);
}
