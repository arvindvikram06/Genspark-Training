// implementation of Iuserinteraction

using NotificationSystem.Interfaces;
using NotificationSystem.Models;
using NotificationSystem.Repositories;

namespace NotificationSystem.Services
{
    internal class UserInteractionService : IUserInteraction
    {
        
        private IRepository<int, User> _userRepository;
        private INotification? _notificationService;

        public UserInteractionService(IRepository<int, User> userRepository)
        {
            _userRepository = userRepository;
        }


        public void AddUser()
        {
            Console.WriteLine("\n----Add New User=----");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";
            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine() ?? "";

            User user = _userRepository.Create(new User(name, email, phone)); // create user object

            Console.WriteLine("User added successfully");
        }

        public void UpdateUser()
        {
            Console.WriteLine("\n----Update User----");
            Console.WriteLine("enter user id that to be updated");
            int id;
            while(!int.TryParse(Console.ReadLine(),out id))
            {
                Console.WriteLine("Invalid Input.. Id must be integer");
            }

            User? user = _userRepository.GetById(id);
            if(user == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            bool exitUpdate = false;
            while (!exitUpdate)
            {
                Console.WriteLine("\n1. Update Phone Number");
                Console.WriteLine("2. Update Email");
                Console.WriteLine("3. Update User Name");
                Console.WriteLine("4. Exit Update");
                Console.Write("Enter choice: ");

                switch (getChoice(1, 4))
                {
                    case 1:
                        Console.WriteLine("enter the phone number");
                        string? phoneNumber = Console.ReadLine();
                        user.PhoneNumber = phoneNumber ?? "";
                        break;

                    case 2:
                        Console.WriteLine("enter the email address");
                        string? email = Console.ReadLine();
                        user.Email = email ?? "";
                        break;
                    case 3:
                        Console.WriteLine("enter the user name");
                        string? userName = Console.ReadLine();
                        user.Name = userName ?? "";
                        break;
                    case 4:
                        exitUpdate = true;
                        break;
                }
            }
            Console.WriteLine(_userRepository.Update(id, user) != null ? "User updated successfully" : "Failed to update user");
        }

        public void DeleteUser()
        {
            Console.WriteLine("\n----Delete User----");
            Console.Write("Enter user ID to delete: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid Input.. Id must be integer");
            }

            User? deletedUser = _userRepository.Delete(id);
            if (deletedUser != null)
            {
                Console.WriteLine($"User with ID {id} deleted successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        public void SendNotification()
        {   
            Console.WriteLine("\n--- Send Notification ---");
            Console.Write("Enter the user ID: ");
            string userIdInput = Console.ReadLine() ?? "";
            
            if(!int.TryParse(userIdInput, out int userId))
            {
                Console.WriteLine("Error: Invalid user ID.");
                return;
            }

            User? user = _userRepository.GetById(userId); // get user
            if (user == null)
            {
                Console.WriteLine("Error: User not found.");
                return;
            }

            Console.Write("Enter the message: ");
            string messageInput = Console.ReadLine() ?? ""; // get message

            Notification notification = new Notification(messageInput);

            if (string.IsNullOrWhiteSpace(messageInput))
            {
                Console.WriteLine("Error: Message cannot be empty.");
                return;
            }

            Console.WriteLine("1. To SendEmail\n2.To SendSms\n");
            
            switch (getChoice(1,2))
            {
                case 1:
                    _notificationService = new EmailNotification();
                    break;
                case 2:
                    _notificationService = new SmsNotification();
                    break;
            }

            _notificationService?.Send(user,notification); // polymorphism
        }

        public void GetAllUser()
        {
            List<User>? users = _userRepository.GetAll();

            if(users == null || users.Count == 0)
            {
                Console.WriteLine("\nUser list is Empty Try adding a user");
                return;
            }

            Console.WriteLine("Users List");
            foreach(User user in users)
                Console.WriteLine(user);
        }


        private int getChoice(int minRange,int maxRange){
            int serviceChoice;
              while(!int.TryParse(Console.ReadLine(),out serviceChoice) || serviceChoice > maxRange || serviceChoice < minRange)
            {
                Console.WriteLine($"Invalid Input enter valid choice between {minRange} and {maxRange}");
            }
            return serviceChoice;
        }



    }
}