using System.Data.Common;

namespace WordGame.Models
{
    public class Game
    {
        public int GameId {get; set;}
        public int UserId {get; set;}
        public string SecretWord {get;}
        public List<Attempt> Attempts {get; set;}

        public int MaxAttempts {get; set;} = 6;

        public int totalScore = 0;

        public Game(string secretWord)
        {
            SecretWord = secretWord;
            Attempts = new List<Attempt>();
        }

        public Game(string secretWord, int gameId, int userId, int maxAttempts, int score)
        {
            SecretWord = secretWord;
            GameId = gameId;
            UserId = userId;
            MaxAttempts = maxAttempts;
            totalScore = score;
            Attempts = new List<Attempt>();
        }

        public void AddAttempt(Attempt attempt)
        {
            Attempts.Add(attempt);
        }

    }
}