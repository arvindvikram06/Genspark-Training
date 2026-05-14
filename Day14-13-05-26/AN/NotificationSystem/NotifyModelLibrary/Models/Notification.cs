using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace NotifyModelLibrary
{
    public class Notification
    {
        public int Id {get;}

        public int UserId { get; set; }
        public string Message { get; set; } = String.Empty;

        [Column(TypeName = "timestamp with time zone")]
        public DateTime SentDateTime { get; set; }
        public string NotificationType {get; set;}

        public User? user { get; set; }

        public Notification()
        {
            SentDateTime = DateTime.UtcNow;
        }

        public Notification(string message,string notificationType, int userId)
        {
            UserId = userId;
            Message = message;
            SentDateTime = DateTime.UtcNow;
            NotificationType = notificationType;
        }

        public Notification(int id,string message,string notificationType, int userId)
        {   
            Id = id;
            UserId = userId;
            Message = message;
            SentDateTime = DateTime.Now;
            NotificationType = notificationType;
        }
    }
}

