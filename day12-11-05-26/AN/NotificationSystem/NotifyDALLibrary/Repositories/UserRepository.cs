//implementation of IRepository
using Npgsql;
using NotifyModelLibrary;
using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary.Exceptions;

namespace NotifyDALLibrary.Repositories
{
    public class UserRepository : AbstractRepository, IRepository<int, User>
    {   

        public User Create(User user)
        {
           NpgsqlConnection _connection = GetDBConnection();
           string query = $"insert into users (name, email, phone_number) values('{user.Name}','{user.Email}','{user.PhoneNumber}')";
           NpgsqlCommand command = new NpgsqlCommand(query,_connection);

            try
            {
                _connection.Open();
                command.ExecuteNonQuery();
                return user;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User? Update(int key, User user)
        {
            NpgsqlConnection _connection = GetDBConnection();
            string query = $"update users set name='{user.Name}',email='{user.Email}',phone_number='{user.PhoneNumber}' where id = '{key}'";
            NpgsqlCommand command = new NpgsqlCommand(query,_connection);
            try
            {
                _connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows > 0 ? user : null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User? GetById(int key)
        {
            NpgsqlConnection _connection = GetDBConnection();
            string query = $"select * from users where id = {key}";
            NpgsqlCommand command = new NpgsqlCommand(query,_connection);

            try
            {
                _connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new User(
                        (int)reader["id"],
                        reader["name"].ToString() ?? string.Empty,
                        reader["email"].ToString() ?? string.Empty,
                        reader["phone_number"].ToString() ?? string.Empty
                    );
                }
            }
            finally
            {
                _connection.Close();
            }
            return null;
        }


        public List<User> GetAll()
        {
            NpgsqlConnection _connection = GetDBConnection();
            List<User> users = new List<User>();
            string query = "select * from Users";
            using NpgsqlCommand command = new NpgsqlCommand(query,_connection);

            try
            {
                _connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User(
                        (int)reader["id"],
                        reader["name"].ToString() ?? string.Empty,
                        reader["email"].ToString() ?? string.Empty,
                        reader["phone_number"].ToString() ?? string.Empty
                    ));
                }

                return users;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User? Delete(int key)
        {   
            NpgsqlConnection _connection = GetDBConnection();
            string query = $"delete from users where id = {key}";
            NpgsqlCommand command = new NpgsqlCommand(query,_connection);
            User? user = GetById(key);
            try
            {
                _connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                if(affectedRows > 0) return user;
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
    
}