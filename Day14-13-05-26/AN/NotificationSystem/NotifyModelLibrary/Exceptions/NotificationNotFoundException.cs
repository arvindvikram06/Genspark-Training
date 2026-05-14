using System;

namespace NotifyModelLibrary.Exceptions
{
    public class NotificationNotFoundException : Exception
    {
        public NotificationNotFoundException(int id) 
            : base($"Notification with ID {id} not found.")
        {
        }
    }
}
