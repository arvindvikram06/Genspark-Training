using Microsoft.EntityFrameworkCore;
using Api.Contexts;
using Api.Models;

namespace Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task AddUsers(IEnumerable<User> users)
        {
            await _context.Users.AddRangeAsync(users);

            await _context.SaveChangesAsync();
        }
    }
}