using System;

namespace NotifyModelLibrary
{
    public class User
    {
        public int Id { get; private set;}
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;

        public bool IsDeleted { get; set; } = false;

        public List<Notification> Notifications = new List<Notification>();
        
        public User(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        

        public User(int id, string name, string email, string phoneNumber)
        {
            Id = id;
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