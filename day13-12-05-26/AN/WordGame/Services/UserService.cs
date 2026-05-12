using System;
using WordGame.Exceptions;
using WordGame.Interfaces;
using WordGame.Models;
using WordGame.Repositories;

namespace WordGame.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository = new UserRepository();
        public User? CurrentUser { get; private set; }

        public User CreateUser(string name, string email, string password)
        {
            try
            {
                User user = new User(name, email, password);
                _userRepository.Create(user);
                return user;
            }
            catch (DatabaseException)
            {
                throw;
            }
        }

        public bool Login(string email, string password)
        {
            try
            {
                User? user = _userRepository.GetByEmail(email);

                if (user == null || user.Password != password)
                {
                    return false;
                }

                CurrentUser = user;
                return true;
            }
            catch (DatabaseException)
            {
                throw;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public User GetByEmail(string email)
        {
            User? user = _userRepository.GetByEmail(email);
            if (user == null) throw new UserNotFoundException(email);
            return user;
        }
    }
}