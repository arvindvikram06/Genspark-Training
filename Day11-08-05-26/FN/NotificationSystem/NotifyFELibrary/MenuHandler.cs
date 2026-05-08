using System;
using NotifyFELibrary.Interfaces;

namespace NotifyFELibrary
{
    public class MenuHandler
    {
        private readonly IUserInteraction _userInteraction;

        public MenuHandler(IUserInteraction userInteraction)
        {
            _userInteraction = userInteraction;
        }

        public void Run()
        {
            while (true)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        UserMenu();
                        break;
                    case "2":
                        NotificationMenu();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("\n=== Notification System ===");
            Console.WriteLine("1. User Management");
            Console.WriteLine("2. Notification Management");
            Console.WriteLine("3. Exit");
            Console.Write("Enter choice: ");
        }

        private void UserMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- User Management ---");
                Console.WriteLine("1.Add User");
                Console.WriteLine("2.List All Users");
                Console.WriteLine("3.Update User");
                Console.WriteLine("4.Delete User");
                Console.WriteLine("5.Back to Main Menu");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine() ?? string.Empty;
                switch (choice)
                {
                    case "1":
                        _userInteraction.AddUser();
                        break;
                    case "2": 
                        _userInteraction.GetAllUser();
                        break;
                    case "3":
                         _userInteraction.UpdateUser();
                        break;
                    case "4": 
                        _userInteraction.DeleteUser();
                         break;
                    case "5": 
                        return;
                    default: Console.WriteLine("Invalid choice.");
                     break;
                }
            }
        }

        private void NotificationMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Notification Management ---");
                Console.WriteLine("1. Send Notification to One User");
                Console.WriteLine("2. Send Notification to All");
                Console.WriteLine("3. List All Notifications");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine() ?? string.Empty;
                switch (choice)
                {
                    case "1": 
                        _userInteraction.SendNotification(); break;
                    case "2": 
                        _userInteraction.SendNotificationToAll();
                    break;
                    case "3":
                         _userInteraction.ListAllNotification();
                          break;
                    case "4": 
                        return;
                    default: 
                         Console.WriteLine("Invalid choice."); 
                         break;
                }
            }
        }
    }
}
