using System;
using NotifyFELibrary.Interfaces; using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;
using NotifyBLLibrary.Services;
using NotifyModelLibrary.Exceptions;

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
                Console.WriteLine($"User created successfully!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void GetUserById()
        {   Console.WriteLine("Enter user id you want to find");
            int id;
            while(!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("id should be integer");
            }
            try
            {
                User user = _userService.GetUserById(id);

                Console.WriteLine("User found");
                Console.WriteLine(user);
            
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
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
                Console.WriteLine($"Notification sent successfully!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void ListAllNotification()
        {
            Console.WriteLine("\n Notification List:\n");

            try
            {
                List<Notification> notifications = _notificationService.GetAllNotifications();
                if (notifications.Count == 0)
                {
                    Console.WriteLine("no notification found\n");
                    return;
                }
                PrintAllNotification(notifications);
            }
            catch (DatabaseException ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        public void GetNotificationById()
        {
            Console.WriteLine("\nGet Notification By ID");
            Console.Write("Enter Notification ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            try
            {
                var notification = _notificationService.GetNotificationById(id);
                PrintNotification(notification);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void UpdateNotification()
        {
            Console.WriteLine("\nUpdate Notification Message");
            Console.Write("Enter Notification ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter New Message: ");
            string message = Console.ReadLine() ?? string.Empty;

            try
            {
                var notification = _notificationService.UpdateNotification(id, message);
                Console.WriteLine("Notification updated successfully!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void DeleteNotification()
        {
            Console.WriteLine("\nUnsend (Delete) Notification");
            Console.Write("Enter Notification ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            try
            {
                _notificationService.DeleteNotification(id);
                Console.WriteLine("Notification unsent successfully!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void PrintNotification(Notification notification)
        {
            Console.WriteLine($"Notification id : {notification.Id}");
            Console.WriteLine($"Sent To User ID: {notification.UserId}");
            Console.WriteLine($"Message : {notification.Message}");
            Console.WriteLine($"Mode of communication : {notification.NotificationType}");
            Console.WriteLine($"Sent At: {notification.SentDateTime}");
            Console.WriteLine("====================");
        }

        private void PrintAllNotification(List<Notification> notifications)
        {

             foreach(Notification notification in notifications)
            {
                PrintNotification(notification);
            }
        }

        private void HandleException(Exception ex)
        {
            switch (ex)
            {
                case ValidationException:
                    Console.WriteLine($"Validation Error: {ex.Message}");
                    break;
                case UserNotFoundException:
                case NotificationNotFoundException:
                    Console.WriteLine($"Not Found: {ex.Message}");
                    break;
                case DatabaseException:
                    Console.WriteLine($"Database error occurred: {ex.Message}");
                    break;
                default:
                    Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                    break;
            }
        }
    }
}
