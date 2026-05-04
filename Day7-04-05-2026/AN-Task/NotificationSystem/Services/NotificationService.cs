using NotificationSystem.Models;
using NotificationSystem.Interfaces;

namespace NotificationSystem.Services
{

    public class NotificationService
    {
        public void SendNotification(INotification notificationType, UserService userService)
        {
            Console.WriteLine("\n--- Send Notification ---");
            Console.Write("Enter the user's email: ");
            string emailInput = Console.ReadLine() ?? "";

            User? user = userService.GetUser(emailInput); // get user
            if (user == null)
            {
                Console.WriteLine("Error: User not found.");
                return;
            }

            Console.Write("Enter the message: ");
            string messageInput = Console.ReadLine() ?? ""; // get message

            if (string.IsNullOrWhiteSpace(messageInput))
            {
                Console.WriteLine("Error: Message cannot be empty.");
                return;
            }

            Notification notification = new Notification(messageInput); // create notification
            notificationType.Send(user, notification); // send notification
        }
    }
}