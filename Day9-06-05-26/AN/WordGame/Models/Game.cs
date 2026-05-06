namespace WordGame.Models
{
    public class Game
    {
        public string SecretWord {get;}
        public List<Attempt> Attempts {get; set;}

        public int MaxAttempts {get; set;} = 6;

        public Game(string secretWord)
        {
            SecretWord = secretWord;
            Attempts = new List<Attempt>();
        }

        public void AddAttempt(Attempt attempt)
        {
            Attempts.Add(attempt);
        }


    }
}