
using WordGame.Interfaces;
using WordGame.Exceptions;

namespace WordGame.Services
{
    public class GuessValidatorService : IGuessValidator
    {
        public void ValidateGuess(string guess)
        {
            if (string.IsNullOrWhiteSpace(guess))
            {
                throw new InvalidGuessException("Input cannot be empty");
            }
            if(guess.Length != 5)
            {
                throw new InvalidGuessException("Word must be 5 letters");
            }
            
             if(guess.Any(c => !char.IsLetter(c)))
            {
                throw new InvalidGuessException("Must only contain letters.");
            }
        }
    }
}