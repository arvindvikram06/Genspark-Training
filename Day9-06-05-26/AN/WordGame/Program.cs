using WordGame.Interfaces;
using WordGame.Repositories;
using WordGame.Services;

namespace WordGame
{
    class Program
    {
        static void Main(string[] args)
        {
            IWordRepository wordRepo = new WordRepository();
            IGuessValidator validator = new GuessValidatorService();
            IFeedBackGenerator feedback = new FeedBackGenerator();

            IGameService gameService =
                new GameService(wordRepo, validator, feedback);

            gameService.StartGame();
        }
    }
}