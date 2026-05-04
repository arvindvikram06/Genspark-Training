using NotificationSystem.Models;

namespace NotificationSystem.Interfaces
{
    public interface INotification
    {
        void Send(User user, Notification notification); 
    }
}