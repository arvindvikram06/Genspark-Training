using System;
using NotificationSystem.Models;
using NotificationSystem.Services;
using NotificationSystem.Interfaces;
using NotificationSystem.Notifications;

namespace NotificationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            NotificationService notificationService = new NotificationService(); 
            UserService userService = new UserService();
            INotification email = new EmailNotification();
            INotification sms = new SmsNotification();

            while (true)
            {
               displayMenu();

                int serviceChoice;

                while (!int.TryParse(Console.ReadLine(), out serviceChoice) || serviceChoice < 1 || serviceChoice > 3)
                {
                    Console.WriteLine("Invalid input enter between 1 and 3");
                }

                switch (serviceChoice)
                {
                    case 1:
                        userService.AddUser();
                        break;
                    case 2:
                        Console.WriteLine("Select Notification Type:\n1. Email\n2. SMS");
                        int typeChoice;
                        while (!int.TryParse(Console.ReadLine(), out typeChoice) || (typeChoice != 1 && typeChoice != 2))
                        {
                            Console.WriteLine("Invalid choice. Enter 1 or 2.");
                        }

                        INotification type = (typeChoice == 1) ? email : sms; // dynamic polymorphism
                        notificationService.SendNotification(type, userService);
                        break;
                    case 3:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void displayMenu()
        {
             Console.WriteLine("\n Notification System Console");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. Send Notification to One User");
                Console.WriteLine("3. Exit");
                Console.Write("Enter choice: ");
        }
    }
}