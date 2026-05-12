using WordGame.Models;
using WordGame.Interfaces;
using WordGame.Exceptions;
using Npgsql;
using System;
using System.Collections.Generic;

namespace WordGame.Repositories
{
    public class GameRepository : AbstractRepository, IRepository<int, Game>
    {
        public Game Create(Game game)
        {
            NpgsqlConnection connection = GetDBConnection();
            string query = $"insert into games (user_id, secret_word, max_attempts, total_score) values ({game.UserId}, '{game.SecretWord}', {game.MaxAttempts}, {game.totalScore}) returning id;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    game.GameId = Convert.ToInt32(result);
                }
                return game;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("error creating game record", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public Game Update(int key, Game game)
        {
            NpgsqlConnection connection = GetDBConnection();
            string query = $"update games set total_score = {game.totalScore}, max_attempts = {game.MaxAttempts} where id = {key};";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                return game;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("error updating game score.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public Game? GetById(int id)
        {
            NpgsqlConnection connection = GetDBConnection();
            string query = $"select * from games where id = {id};";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Game(reader["secret_word"].ToString() ?? string.Empty,
                        (int)reader["id"],
                        (int)reader["user_id"],
                        (int)reader["max_attempts"],
                        (int)reader["total_score"]
                    );
                }
                return null;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("error retrieving game details.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Game> GetAll()
        {
            List<Game> games = new List<Game>();
            NpgsqlConnection connection = GetDBConnection();
            string query = "select * from games;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Game game = new Game(reader["secret_word"].ToString() ?? string.Empty,
                        (int)reader["id"],
                        (int)reader["user_id"],
                        (int)reader["max_attempts"],
                        (int)reader["total_score"]
                    );
                    games.Add(game);
                }
                return games;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("error retrieving game list.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

//future implementation
        public Game? Delete(int key) => throw new NotImplementedException();
    }
}