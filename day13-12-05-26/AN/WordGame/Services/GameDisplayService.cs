using System;
using WordGame.Interfaces;
using WordGame.Models;

namespace WordGame.Services
{
    public class GameDisplayService : IGameDisplayService
    {
        public void PrintFeedback(Attempt attempt)
        {
            foreach (char c in attempt.FeedBack)
            {
                Console.ForegroundColor =
                    c == 'G' ? ConsoleColor.Green :
                    c == 'Y' ? ConsoleColor.Yellow :
                               ConsoleColor.Red;

                Console.Write(c + " ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        public void PrintGameHistory(Game game)
        {
            Console.WriteLine("\nGame History:");
            foreach (var attempt in game.Attempts)
            {
                Console.Write(
                    $"Attempt: {attempt.AttemptNumber}\n" +
                    $"Guess:\n{string.Join(" ", attempt.Guess.ToCharArray())}\n"
                );
                PrintFeedback(attempt);
            }
        }

        public void PrintComment(int attempt)
        {
            string[] comments =
            {
                "Genius!",
                "Excellent!",
                "Great job!",
                "Good work!",
                "Nice try!",
                "That was close!"
            };

            if (attempt > 0 && attempt <= comments.Length)
            {
                Console.WriteLine(comments[attempt - 1]);
            }
        }

        public void DisplayScore(int score)
        {
            Console.WriteLine($"Your score is : {score}");
        }

        public void DisplayMenu()
        {
            Console.WriteLine("\n=== User Menu ===");
            Console.WriteLine("1. Play Game");
            Console.WriteLine("2. List all my games");
            Console.WriteLine("3. Get game history (by ID)");
            Console.WriteLine("4. Logout");
        }
    }
}
