//implementation of IRepository
using Npgsql;
using NotifyModelLibrary;
using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary.Exceptions;
using NotifyDALLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace NotifyDALLibrary.Repositories
{
    public class UserRepository : IRepository<int, User>
    {   
        private readonly NotifyContext _context;


        public UserRepository(NotifyContext context)
        {
             _context = context;
        }

        public User Create(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Could not create user in database", ex);
            }
        }

        public User? Update(int key, User user)
        {
            try
            {
                var updatedEntity = _context.Users.Update(user);
                _context.SaveChanges();
                return updatedEntity.Entity;
            }
            catch (DbUpdateException ex)
            {
                 throw new DatabaseException("Could not update user in database", ex);
            }
        }

        public User? GetById(int key)
        {
            try
            {
                return _context.Users.Find(key);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error retrieving user with ID {key}", ex);
            }
        }


        public List<User> GetAll()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error retrieving all users", ex);
            }
        }


        public User? Delete(int key)
        {   
            try
            {
                User? user = GetById(key);

                if(user == null)
                    return null;

                user.IsDeleted = true;
                _context.Users.Update(user);
                _context.SaveChanges();
                return user;
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Could not delete user in database", ex);
            }
        }
    }
    
}