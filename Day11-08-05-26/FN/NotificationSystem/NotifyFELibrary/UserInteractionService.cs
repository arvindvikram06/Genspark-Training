using System;
using NotifyFELibrary.Interfaces; using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;
using NotifyBLLibrary.Services;

namespace NotifyFELibrary
{
    public class UserInteractionService : IUserInteraction
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public UserInteractionService(UserService userService, NotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        public void AddUser()
        {
            Console.WriteLine("\nAdd User");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine() ?? string.Empty;

            try
            {
                var user = _userService.CreateUser(name, email, phone);
                Console.WriteLine($"User created successfully! ID: {user.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void GetAllUser()
        {
            Console.WriteLine("\nAll Users:");
            try
            {
                var users = _userService.GetAllUsers();
                if (users.Count == 0)
                {
                    Console.WriteLine("No users found.");
                    return;
                }

                foreach (var user in users)
                {
                    Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Email: {user.Email}, Phone: {user.PhoneNumber}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateUser()
        {
            Console.WriteLine("\nUpdate User");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User ID.");
                return;
            }

            Console.Write("Enter Property to update name or email or phone: ");
            string property = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter New Value: ");
            string value = Console.ReadLine() ?? string.Empty;

            try
            {
                var user = _userService.UpdateUser(userId, property, value);
                Console.WriteLine("User updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteUser()
        {
            Console.WriteLine("\nDelete User");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User ID.");
                return;
            }
            try
            {
                var user = _userService.DeleteUser(userId);
                Console.WriteLine("User deleted successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void SendNotification()
        {
            Console.WriteLine("\nSend Notification:");
            Console.Write("Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User ID.");
                return;
            }

            Console.Write("Enter Notification Type email or sms: ");
            string type = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter Message: ");
            string message = Console.ReadLine() ?? string.Empty;

            try
            {
                var notification = _notificationService.SendNotifification(userId, type, message);
                Console.WriteLine($"Notification sent successfully! ID: {notification.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public void SendNotificationToAll()
        {
            try
            {   
                Console.WriteLine("\nSend Notification to All:\n");
                Console.WriteLine("Enter the message you want to send");
            
                string message = Console.ReadLine() ?? string.Empty;
                
                Console.WriteLine("\nEnter the Mode of communication : type email or sms");
                string type = Console.ReadLine() ?? string.Empty;

                _notificationService.SendNotificationToAll(type,message);
                
                Console.WriteLine("Notifcation Sent Successfully!!");

            }
            catch(Exception ex)
            {
                Console.Write($"Failed to send : {ex.Message}");
            }
        }

        public void ListAllNotification()
        {
            Console.WriteLine("\n Notification List:\n");

            List<Notification> notifications = _notificationService.GetAllNotifications();
            if(notifications.Count == 0)
            {
                Console.WriteLine("no notification found\n");
                return;
            }
            PrintAllNotification(notifications);
           
        }

        private void PrintAllNotification(List<Notification> notifications)
        {

             foreach(Notification notification in notifications)
            {
                Console.WriteLine($"Notification id : {notification.Id}");
                Console.WriteLine($"Sent To User ID: {notification.UserId}");
                Console.WriteLine($"Message : {notification.Message}");
                Console.WriteLine($"Mode of communication : {notification.NotificationType}");
                Console.WriteLine($"Sent At: {notification.SentDateTime}");
                Console.WriteLine("====================");
            }
        }
    }
}
