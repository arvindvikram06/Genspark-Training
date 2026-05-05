using System;
using NotificationSystem.Interfaces;
using NotificationSystem.Models;

namespace NotificationSystem.Services
{
    internal class EmailNotification : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine("====================");
            Console.WriteLine($"Email sent to {user.Email}\nWith message: {notification.Message}\nAt {notification.SentDateTime}");
            Console.WriteLine("====================");

        }
    }
}