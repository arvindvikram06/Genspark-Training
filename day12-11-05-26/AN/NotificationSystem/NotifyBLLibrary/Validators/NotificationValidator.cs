using NotifyModelLibrary;
using NotifyModelLibrary.Exceptions;

namespace NotifyBLLibrary.Validators;

public static class NotificationValidator
{
    public static void ValidateNotification(string type,string message)
    {
        ValidateMessage(message);
        ValidateNotificationType(type);
    }
    public static void ValidateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ValidationException("message cannot be empty");
        if (message.Length < 5)
            throw new ValidationException("message must be at least 5 characters long");
        if (message.Length > 160)
            throw new ValidationException("message cannot exceed 100 characters");
    }

    public static void ValidateNotificationType(string type)
    {
        var validTypes = new[] { "email", "sms" }; 
        if (string.IsNullOrWhiteSpace(type))
            throw new ValidationException("Notification type cannot be empty");

        if (!validTypes.Contains(type.ToLower()))
            throw new ValidationException($"notification type Must be one of: {string.Join(", ", validTypes)}");
    }
}
