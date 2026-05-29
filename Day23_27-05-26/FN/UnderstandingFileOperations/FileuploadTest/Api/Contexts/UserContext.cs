using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
    }
}