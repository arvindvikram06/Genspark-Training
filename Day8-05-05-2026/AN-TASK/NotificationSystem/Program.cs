using System;
using NotificationSystem.Models;
using NotificationSystem.Services;
using NotificationSystem.Interfaces;
using NotificationSystem.Repositories;

namespace NotificationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, User> userRepository = new UserRepository();
            IUserInteraction userInteraction = new UserInteractionService(userRepository);

            while (true)
            {
               displayMenu();

                int serviceChoice;

                while (!int.TryParse(Console.ReadLine(), out serviceChoice) || serviceChoice < 1 || serviceChoice > 6)
                {
                    Console.WriteLine("Invalid input enter between 1 and 6");
                }

                switch (serviceChoice)
                {
                    case 1:
                        userInteraction.AddUser();
                        break;
                    case 2:
                        userInteraction.SendNotification();
                        break;
                    case 3:
                        userInteraction.GetAllUser();
                        break;
                    case 4:
                        userInteraction.UpdateUser();
                        break;
                    case 5:
                        userInteraction.DeleteUser();
                        break;
                    case 6:
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
                Console.WriteLine("3. List All Users");
                Console.WriteLine("4. Update User");
                Console.WriteLine("5. Delete User");
                Console.WriteLine("6. Exit");

                Console.Write("Enter choice: ");
        }
    }
}