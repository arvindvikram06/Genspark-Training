using System;
using System.Collections.Generic;
using WordGame.Interfaces;
using WordGame.Models;
using WordGame.Exceptions;
using WordGame.Repositories;
using System.Linq;

namespace WordGame.Services
{
    public class GameService : IGameService
    {
        private readonly IWordRepository _wordRepo;
        private readonly IUserService _userService;
        private readonly GameRepository _gameRepo;
        private readonly AttemptRepository _attemptRepo;
        
        private readonly IGameDisplayService _display;
        private readonly IGameHistoryService _history;

        public GameService(IWordRepository wordRepo, IUserService userService)
        {
            _wordRepo = wordRepo;
            _userService = userService;
            _gameRepo = new GameRepository();
            _attemptRepo = new AttemptRepository();
            
            _display = new GameDisplayService();
            _history = new GameHistoryService(_gameRepo, _attemptRepo, _display);
        }

        public void StartGame()
        {
            bool inDashboard = true;
            while (inDashboard)
            {
                _display.DisplayMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        PlayNewGame();
                        break;
                    case "2":
                        if (_userService.CurrentUser != null)
                            _history.ListAllGames(_userService.CurrentUser.Id);
                        break;
                    case "3":
                        _history.ViewGameHistoryById();
                        break;
                    case "4":
                        _userService.Logout();
                        inDashboard = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void PlayNewGame()
        {
            if (_userService.CurrentUser == null)
            {
                Console.WriteLine("You must be logged in to play.");
                return;
            }

            Game game = new Game(_wordRepo.GetRandomWord());
            game.UserId = _userService.CurrentUser.Id;
            game = _gameRepo.Create(game);

            HashSet<string> guesses = new HashSet<string>();
            Console.WriteLine("\n=== New Game ===");
            Console.WriteLine("Find the word!");

            for (int i = 1; i <= game.MaxAttempts; i++)
            {
                Console.WriteLine($"Attempt {i}: ");
                string guess = Console.ReadLine()?.ToUpper() ?? string.Empty;

                try
                {
                    GameHelper.ValidateGuess(guess);

                    if (guesses.Contains(guess))
                    {
                        Console.WriteLine("Duplicate guess!");
                        i--;
                        continue;
                    }

                    guesses.Add(guess);
                    string feedback = GameHelper.GenerateFeedback(game.SecretWord, guess);
                    
                    Attempt attempt = new Attempt(guess, feedback, i.ToString());
                    attempt.GameId = game.GameId;

                    game.AddAttempt(attempt);
                    _attemptRepo.Create(attempt);
                    _display.PrintFeedback(attempt);

                    if (guess.Equals(game.SecretWord))
                    {
                        _display.PrintComment(i);
                        game.totalScore = CalculateAndDisplayScore(game.Attempts.Count - 1);
                        _gameRepo.Update(game.GameId, game);
                        break;
                    }
                    if (i == game.MaxAttempts)
                    {
                        Console.WriteLine($"word was: {game.SecretWord}");
                        _display.PrintComment(i);
                        game.totalScore = CalculateAndDisplayScore(game.Attempts.Count);
                        _gameRepo.Update(game.GameId, game);
                    }
                }
                catch (InvalidGuessException ex)
                {
                    Console.WriteLine(ex.Message);
                    i--;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"an unexpected error occurred: {ex.Message}");
                    i--;
                }
            }
            Console.WriteLine("\nGame Over! Returning to User Menu...");
        }

        private int CalculateAndDisplayScore(int attempts)
        {
            int score = 100 - (attempts * 10);
            _display.DisplayScore(score);
            return score;
        }
    }
}