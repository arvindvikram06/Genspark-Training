//implementation of IRepository

using NotifyModelLibrary;
using NotifyDALLibrary.Interfaces;

namespace NotifyDALLibrary.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        private Dictionary<int, User> _userList = new Dictionary<int, User>();

        public UserRepository()
        {
            User user = new User("Arvind", "arvind@gmail.com", "1234567890");
            _userList.Add(user.Id,user);
        }

        public User Create(User user)
        {
            _userList.Add(user.Id, user);
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

        public List<User> GetAll()
        {
            return _userList.Values.ToList();
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