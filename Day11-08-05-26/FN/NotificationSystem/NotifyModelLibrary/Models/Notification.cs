using System;
namespace NotifyModelLibrary
{
    public class Notification
    {
        private static int count = 0;
        public int UserId { get; set; }
        public int Id {get;}
        public string Message { get; set; } = String.Empty;
        public DateTime SentDateTime { get; set;}
        public string NotificationType {get; set;}
        public Notification(string message,string notificationType, int userId)
        {
            Id = ++count;
            UserId = userId;
            Message = message;
            SentDateTime = DateTime.Now;
            NotificationType = notificationType;
        }
    }
}