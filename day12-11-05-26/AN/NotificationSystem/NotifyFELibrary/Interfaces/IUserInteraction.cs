// interface for user interaction
// provides flexiblity to change the UserInterface to any platform without modifying the BLL
namespace NotifyFELibrary.Interfaces
{
    public interface IUserInteraction
    {
        void AddUser();

        void UpdateUser();

        void GetAllUser();

        void GetUserById();
        
        void DeleteUser();

        void SendNotification();

        void SendNotificationToAll();

        void ListAllNotification();

        void GetNotificationById();

        void UpdateNotification();

        void DeleteNotification();
        
    }
}