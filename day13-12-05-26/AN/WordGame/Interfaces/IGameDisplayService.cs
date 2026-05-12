using WordGame.Models;

namespace WordGame.Interfaces
{
    public interface IGameDisplayService
    {
        void PrintFeedback(Attempt attempt);
        void PrintGameHistory(Game game);
        void PrintComment(int attempt);
        void DisplayScore(int score);
        void DisplayMenu();
    }
}
