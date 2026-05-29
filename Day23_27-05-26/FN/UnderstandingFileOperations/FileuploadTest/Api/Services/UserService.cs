using MiniExcelLibs;
using Api.Models;
using Api.Repositories;
using Api.Models.DTO;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _repository.GetAllUsers();
        }

        public async Task<User> CreateUser(User user)
        {
            return await _repository.AddUser(user);
        }

        public async Task<string> ExportUsersToExcel()
        {
            var users = await _repository.GetAllUsers();

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Exports");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"Users_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            var filePath = Path.Combine(folderPath, fileName);

            await MiniExcel.SaveAsAsync(filePath, users);

            return filePath;
        }


        public async Task UploadUsersFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            using var stream = file.OpenReadStream();

            var uploads = stream.Query<UserRequest>().ToList();

            var users = uploads.Select( x => new User
            {
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                Age = x.Age
            });

            await _repository.AddUsers(users);
        }
    }
}