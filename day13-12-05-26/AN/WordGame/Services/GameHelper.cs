using System;
using System.Linq;
using WordGame.Exceptions;

namespace WordGame.Services
{
    public static class GameHelper
    {
        public static void ValidateGuess(string guess)
        {
            if (string.IsNullOrWhiteSpace(guess))
            {
                throw new InvalidGuessException("Input cannot be empty");
            }
            if (guess.Length != 5)
            {
                throw new InvalidGuessException("Word must be 5 letters");
            }
            if (guess.Any(c => !char.IsLetter(c)))
            {
                throw new InvalidGuessException("Must only contain letters.");
            }
        }

        public static string GenerateFeedback(string secretWord, string guess)
        {
            secretWord = secretWord.ToUpper();
            guess = guess.ToUpper();
            
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
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
        }
    }
}
