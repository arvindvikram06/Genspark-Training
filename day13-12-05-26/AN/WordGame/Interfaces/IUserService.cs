using WordGame.Models;

namespace WordGame.Interfaces
{
    public interface IUserService
    {
        User? CurrentUser { get; }
        User CreateUser(string name, string email, string password);
        bool Login(string email, string password);
        void Logout();
    }
}