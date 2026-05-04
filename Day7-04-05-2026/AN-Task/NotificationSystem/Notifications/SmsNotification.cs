using System;
using NotificationSystem.Interfaces;
using NotificationSystem.Models;

namespace NotificationSystem.Notifications
{
    public class SmsNotification : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine("====================");
            Console.WriteLine($"SMS sent to {user.PhoneNumber}\nWith message: {notification.Message}\nAt {notification.SentDateTime}");
            Console.WriteLine("====================");
        }
    }
}