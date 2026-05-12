namespace WordGame.Models
{
    public class User
    {
        public int Id {get;}
        public string Name {get; set;}
        public string Password {get; set;}
        public string Email {get; set;}

        public User(string username ,string email,string password)
        {
            Name = username;
            Password = password;
            Email = email;
        }

        public User(int id,string username,string email,string password)
        {
            Id = id;
            Name = username;
            Password = password;
            Email = email;

        }


        public override string ToString()
        {
            return $"User name : {Name}";
        }
    }
}