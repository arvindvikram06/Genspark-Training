
using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;
using NotifyModelLibrary.Exceptions;
using NotifyDALLibrary.Repositories;
using NotifyBLLibrary.Validators;

namespace NotifyBLLibrary.Services
{
    public class UserService
    {
        private IRepository<int, User> _userRepository;

        public UserService(IRepository<int, User> userRepository)
        {
            _userRepository = userRepository;
        }

        public User CreateUser(string name, string email, string phoneNumber)
        {
            UserValidator.ValidateUserDetails(name, email, phoneNumber); // validate user inputs
            User user = new User(name, email, phoneNumber); 
            return _userRepository.Create(user); // save user to repository
        }
        public User GetUserById(int id)
        {
            User? user = _userRepository.GetById(id);
            if (user == null)
                throw new UserNotFoundException(id);
            return user;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = _userRepository.GetAll();
            if (users.Count == 0)
                throw new UserNotFoundException();
            return users;
        }

        public User UpdateUser(int userId, string property, string value)
        {
            User user = GetUserById(userId);

            switch (property.ToLower()) // update user property based on user input
            {
                case "name":
                    UserValidator.ValidateName(value);
                    user.Name = value;
                    break;
                case "email":
                    UserValidator.ValidateEmail(value);
                    user.Email = value;
                    break;
                case "phone":
                    UserValidator.ValidatePhone(value);
                    user.PhoneNumber = value;
                    break;
                default:
                    throw new ValidationException($"Unknown property: {property}. Valid properties: name, email, phone");
            }

            User? updatedUser = _userRepository.Update(userId, user);
            if (updatedUser == null)
                throw new Exception($"Failed to update user property.");

            return updatedUser;
        }

        public User DeleteUser(int userId)
        {
            User? user = _userRepository.Delete(userId);
            if(user == null)
            {
                throw new UserNotFoundException(userId);
            }
            return user;
        }
    }
}