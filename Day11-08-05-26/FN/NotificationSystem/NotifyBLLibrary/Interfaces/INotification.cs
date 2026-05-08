// interface for notification

using NotifyModelLibrary;

namespace NotifyBLLibrary.Interfaces
{
    public interface INotification
    {
        void Send(User user, Notification notification);
    }
}