using Api.Models;

namespace Api.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();

        Task<User> AddUser(User user);

        Task AddUsers(IEnumerable<User> users);
    }
}