using System;
using System.Collections.Generic;
using WordGame.Models;
using WordGame.Interfaces;
using WordGame.Exceptions;
using Npgsql;

namespace WordGame.Repositories
{
    public class AttemptRepository : AbstractRepository
    {
        public void Create(Attempt attempt)
        {
            NpgsqlConnection connection = GetDBConnection();
            string timeString = attempt.AttemptTime.ToString("yyyy-MM-dd HH:mm:ss");
            string query = $@"insert into attempts (game_id, guess, feedback, attempt_number, result, attempt_time) 
                             values ({attempt.GameId}, '{attempt.Guess}', '{attempt.FeedBack}', 
                             '{attempt.AttemptNumber}', {attempt.result}, '{timeString}');";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("Error saving game attempt.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Attempt> GetAttemptsByGameId(int gameId)
        {
            List<Attempt> attempts = new List<Attempt>();
            NpgsqlConnection connection = GetDBConnection();
            string query = $"select * from attempts where game_id = {gameId} order by attempt_time ASC;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Attempt attempt = new Attempt(
                        reader["guess"].ToString() ?? string.Empty,
                        reader["feedback"].ToString() ?? string.Empty,
                        reader["attempt_number"].ToString() ?? string.Empty,
                        (bool)reader["result"]
                    );
                    attempt.AttemptTime = (DateTime)reader["attempt_time"];
                    attempt.GameId = (int)reader["game_id"];
                    attempts.Add(attempt);
                }
                return attempts;
            }
            catch (NpgsqlException ex)
            {
                throw new DatabaseException("Error retrieving attempts for the game.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}