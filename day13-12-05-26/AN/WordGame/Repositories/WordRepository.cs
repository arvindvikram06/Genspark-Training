using System;
using System.Collections.Generic;
using Npgsql;
using WordGame.Interfaces;
using WordGame.Models;


namespace WordGame.Repositories
{
    public class WordRepository : AbstractRepository , IWordRepository
    {
        // private readonly List<string> _words = new List<string>
        // {
        //     "APPLE", "MANGO", "GRAPE", "TRAIN", "PLANT", "BRAIN"
        // };

        public string GetRandomWord()
        {
           NpgsqlConnection _connection = GetDBConnection();

           string query = $"select * from words order by random() limit 1";

           NpgsqlCommand command = new NpgsqlCommand(query,_connection);

            try
            {
                _connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                string word = string.Empty;
                if (reader.Read())
                {
                    word = reader["word"].ToString() ?? string.Empty;
                }
                reader.Close();
                return word;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}