// sms implementation of INotification
using System;
using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;

namespace NotifyBLLibrary.NotificationSenders
{
    internal class SmsNotificationSender : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine("====================");
            Console.WriteLine($"SMS sent to {user.PhoneNumber}\nWith message: {notification.Message}\nAt {notification.SentDateTime}");
            Console.WriteLine("====================");
        }
    }
}