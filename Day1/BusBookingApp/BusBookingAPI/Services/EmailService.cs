using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace BusBookingAPI.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendBookingConfirmationEmailAsync(string toEmail, string userName, int bookingId, string busName, string route, string departureTime, decimal amount)
    {
        try
        {
            var subject = "Booking Confirmation - Bus Booking";
            var body = $@"
Dear {userName},

Your booking has been confirmed successfully!

Booking Details:
- Booking ID: #{bookingId}
- Bus: {busName}
- Route: {route}
- Departure Time: {departureTime}
- Total Amount: Rs {amount}

Thank you for booking with us. Have a safe journey!

Best regards,
Bus Booking Team";

            await SendEmailAsync(toEmail, subject, body);
            _logger.LogInformation("Booking confirmation email sent to {Email} for booking {BookingId}", toEmail, bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking confirmation email to {Email} for booking {BookingId}", toEmail, bookingId);
        }
    }

    public async Task SendPaymentFailureEmailAsync(string toEmail, string userName, int bookingId, string failureReason)
    {
        try
        {
            var subject = "Payment Failed - Bus Booking";
            var body = $@"
Dear {userName},

We regret to inform you that your payment could not be processed.

Booking Details:
- Booking ID: #{bookingId}

Reason: {failureReason}

Please try again or contact support for assistance.

Best regards,
Bus Booking Team";

            await SendEmailAsync(toEmail, subject, body);
            _logger.LogInformation("Payment failure email sent to {Email} for booking {BookingId}", toEmail, bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment failure email to {Email} for booking {BookingId}", toEmail, bookingId);
        }
    }

    public async Task SendCancellationEmailAsync(string toEmail, string userName, string source, string destination, DateTime departureTime, decimal amount)
    {
        try
        {
            var subject = "Booking Cancelled - Bus Booking";
            // Convert to local time for display (departureTime is already local from caller)
            var localTime = departureTime.Kind == DateTimeKind.Utc
                ? departureTime.ToLocalTime()
                : departureTime;
            var body = $@"
Dear {userName},

Your booking has been successfully cancelled.

Booking Details:
- Route: {source} to {destination}
- Departure Time: {localTime:yyyy-MM-dd HH:mm}
- Amount: Rs {amount}

The seats have been released and are now available for booking.

If you did not request this cancellation, please contact our support team immediately.

Best regards,
Bus Booking Team";

            await SendEmailAsync(toEmail, subject, body);
            _logger.LogInformation("Cancellation email sent to {Email} for route {Source} to {Destination}", toEmail, source, destination);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancellation email to {Email}", toEmail);
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            EnableSsl = _smtpSettings.EnableSsl,
            Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}
