// interface for user interaction
namespace NotificationSystem.Interfaces
{
    public interface IUserInteraction
    {
        void AddUser();

        void UpdateUser();

        void GetAllUser();

        void SendNotification();

        void DeleteUser();
    }
}