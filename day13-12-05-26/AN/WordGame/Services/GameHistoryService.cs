using System;
using System.Linq;
using WordGame.Interfaces;
using WordGame.Repositories;

namespace WordGame.Services
{
    public class GameHistoryService : IGameHistoryService
    {
        private readonly GameRepository _gameRepo;
        private readonly AttemptRepository _attemptRepo;
        private readonly IGameDisplayService _display;

        public GameHistoryService(GameRepository gameRepo, AttemptRepository attemptRepo, IGameDisplayService display)
        {
            _gameRepo = gameRepo;
            _attemptRepo = attemptRepo;
            _display = display;
        }

        public void ListAllGames(int userId)
        {
            var allMyGames = _gameRepo.GetAll().Where(g => g.UserId == userId).ToList();
            Console.WriteLine("\n=== All My Games ===");
            if (allMyGames.Count == 0)
            {
                Console.WriteLine("No games found.");
            }
            else
            {
                foreach (var g in allMyGames)
                {
                    Console.WriteLine($"Game ID: {g.GameId} | Score: {g.totalScore} | Max Attempts: {g.MaxAttempts}");
                }
            }
            Console.WriteLine("====================\n");
        }

        public void ViewGameHistoryById()
        {
            Console.WriteLine("Enter Game ID to view history:");
            if (int.TryParse(Console.ReadLine(), out int gameId))
            {
                var gameFromDb = _gameRepo.GetById(gameId);
                if (gameFromDb != null)
                {
                    gameFromDb.Attempts = _attemptRepo.GetAttemptsByGameId(gameId);
                    _display.PrintGameHistory(gameFromDb);
                }
                else
                {
                    Console.WriteLine("Could not find game in database.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a numeric Game ID.");
            }
        }
    }
}
