namespace WordGame.Interfaces
{
    public interface IFeedBackGenerator
    {
        string GenerateFeedback(string secretWord, string guess);     
    }
}