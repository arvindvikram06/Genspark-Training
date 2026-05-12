
namespace WordGame.Models
{
    public class Attempt
    {
        public int AttemptId {get; set;}
        public int GameId {get; set;}
        public string Guess {get; set;}
        public string FeedBack {get; set;}

        public bool result{get;} = false;

        public string AttemptNumber {get; set;}

        public DateTime  AttemptTime{get; set;}


        public Attempt(string guess,string feedback,string attemptnumber)
        {
            Guess = guess;
            FeedBack = feedback;
            AttemptNumber = attemptnumber;
            AttemptTime = DateTime.Now;
        }

        public Attempt(string guess,string feedback,string attemptnumber,bool result)
        {
            Guess = guess;
            FeedBack = feedback;
            AttemptNumber = attemptnumber;
            AttemptTime = DateTime.Now;
            result = true;
        }

    }
}