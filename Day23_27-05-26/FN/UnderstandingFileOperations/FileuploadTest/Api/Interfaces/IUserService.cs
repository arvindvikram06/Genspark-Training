using Api.Models;

namespace Api.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();

        Task<User> CreateUser(User user);

        Task<string> ExportUsersToExcel();

        Task UploadUsersFromExcel(IFormFile file);
    }
}