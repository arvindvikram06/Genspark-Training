using System;
namespace NotificationSystem.Models
{
    public class Notification
    {
        public string Message { get; set; } = String.Empty;
        public DateTime SentDateTime { get; set; }

        public Notification(string message)
        {
            Message = message;
            SentDateTime = DateTime.Now;
        }
    }
}