
namespace WordGame.Models
{
    public class Attempt
    {
        public string Guess {get; set;}
        public string FeedBack {get; set;}

        public string AttemptNumber {get; set;}

        public DateTime  AttemptTime{get; set;}


        public Attempt(string guess,string feedback,string attemptnumber)
        {
            Guess = guess;
            FeedBack = feedback;
            AttemptNumber = attemptnumber;
            AttemptTime = DateTime.Now;
        }

    }
}