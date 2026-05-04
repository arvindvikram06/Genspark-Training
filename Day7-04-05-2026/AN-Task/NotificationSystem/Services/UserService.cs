using System;
using System.Collections.Generic;
using NotificationSystem.Models;

namespace NotificationSystem.Services
{
    public class UserService
    {
        private List<User> _users = new List<User>();

        public void AddUser()
        {
            Console.WriteLine("\n----Add New User=----");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";
            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine() ?? "";

            User user = new User(name, email, phone); // create user object
            _users.Add(user); // add to list
            Console.WriteLine("User added successfully");
        }

        public User? GetUser(string email)
        {
            foreach (User user in _users)
            {
                if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }
            return null;
        }
    }
}