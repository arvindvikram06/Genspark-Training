using WordGame.Models;
using WordGame.Interfaces;
using WordGame.Exceptions;
using Npgsql;
using System;
using System.Collections.Generic;

namespace WordGame.Repositories
{
    public class UserRepository : AbstractRepository, IRepository<int, User>
    {
        public User Create(User user)
        {
            NpgsqlConnection connection = GetDBConnection();
            string query = $"insert into users (name, email, password) values ('{user.Name}', '{user.Email}', '{user.Password}')";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                return user;
            }
            catch (NpgsqlException ex)
            {   
                
                if (ex.SqlState == "23505")
                    throw new DatabaseException("a user with this email already exists", ex);

                throw new DatabaseException("A db error occurred during user creation", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public User? GetByEmail(string email)
        {
            NpgsqlConnection connection = GetDBConnection();
            string query = $"select * from users where email = '{email}'";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User(
                        (int)reader["id"],
                        reader["name"].ToString() ?? string.Empty,
                        reader["email"].ToString() ?? string.Empty,
                        reader["password"].ToString() ?? string.Empty
                    );
                }
                return null;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("failed to retrieve user by email", ex);
            }
            finally
            {
                connection.Close();
            }
        }


//future implementation
        public User? Delete(int key) => throw new NotImplementedException();
        public List<User> GetAll() => throw new NotImplementedException();
        public User? GetById(int id) => throw new NotImplementedException();
        public User? Update(int id, User obj) => throw new NotImplementedException();
    }
}