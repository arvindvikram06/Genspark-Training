// email implementation of INotification
using System;
using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;

namespace NotifyBLLibrary.NotificationSenders
{
    internal class EmailNotificationSender : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine("====================");
            Console.WriteLine($"Email sent to {user.Email}\nWith message: {notification.Message}\nAt {notification.SentDateTime}");
            Console.WriteLine("====================");

        }
    }
}