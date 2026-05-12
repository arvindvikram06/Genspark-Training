namespace WordGame.Models
{
    public class Word
    {
        public int Id {get; set;}

        public string WordG {get; set;} = string.Empty;

        public Word(int id,string word)
        {
            Id = id;
            word = WordG;
        }

    }
}