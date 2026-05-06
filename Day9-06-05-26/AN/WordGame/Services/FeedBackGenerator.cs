
using WordGame.Interfaces;

namespace WordGame.Services
{
    public class FeedBackGenerator : IFeedBackGenerator
    {
       public string GenerateFeedback(string secretWord, string guess)
{
    char[] result = new char[secretWord.Length];
    char[] secretChars = secretWord.ToCharArray();

    for (int i = 0; i < secretWord.Length; i++)
    {
        if (guess[i] == secretWord[i])
        {
            result[i] = 'G';
            secretChars[i] = '*'; 
        }
    }

    for (int i = 0; i < secretWord.Length; i++)
    {
        if (result[i] == 'G')
            continue;

        int index = Array.IndexOf(secretChars, guess[i]);

        if (index != -1)
        {
            result[i] = 'Y';
            secretChars[index] = '*';
        }
        else
        {
            result[i] = 'X';
        }
    }

    return new string(result);
}

    }
}