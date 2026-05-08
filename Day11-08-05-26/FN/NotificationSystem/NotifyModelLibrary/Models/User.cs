using System;

namespace NotifyModelLibrary
{
    public class User
    {
        private static int count = 0;
        public int Id { get; private set;}
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;

        public User(string name, string email, string phoneNumber)
        {
            Id = ++count;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Email: {Email}, Phone: {PhoneNumber}";
        }
    }
}