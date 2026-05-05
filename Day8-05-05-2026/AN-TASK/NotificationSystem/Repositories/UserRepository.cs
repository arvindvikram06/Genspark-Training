//implementation of IRepository

using NotificationSystem.Models;
using NotificationSystem.Interfaces;

namespace NotificationSystem.Repositories
{
    internal class UserRepository : IRepository<int, User>
    {
        private static int id = 1;
        private Dictionary<int, User> _userList = new Dictionary<int, User>();

        public UserRepository()
        {
            _userList.Add(id, new User("Arvind", "arvind@gmail.com", "1234567890") { Id = id++});
            _userList.Add(id, new User("vikram", "vikram@gmail.com", "1234567891") { Id = id++});
            _userList.Add(id, new User("vijay", "vijay@gmail.com", "1234567892") { Id = id++});
        }

        public User Create(User user)
        {
            user.Id = id;
            _userList.Add(id, user);
            id++;
            return user;
        }

        public User? Update(int key, User user)
        {
            if (_userList.ContainsKey(key) && user != null)
            {
                _userList[key] = user;
                return user;
            }
            return null;
        }

        public User? GetById(int key)
        {
            if (_userList.ContainsKey(key))
            {
                return _userList[key];
            }
            return null;
        }

        public List<User>? GetAll()
        {
            if (_userList.Count > 0)
            {
                return _userList.Values.ToList();
            }
            return null;
        }

        public User? Delete(int key)
        {
            if (_userList.ContainsKey(key))
            {
                User user = _userList[key];
                _userList.Remove(key);
                return user;
            }
            return null;
        }
    }
    
}