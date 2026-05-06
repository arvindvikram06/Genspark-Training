using System;
using System.Collections.Generic;
using WordGame.Interfaces;
using WordGame.Models;
using WordGame.Exceptions;
using WordGame.Repositories;
namespace WordGame.Services
{
    public class GameService : IGameService
    {
        private readonly IWordRepository _wordRepo;
        private readonly IGuessValidator _validator;
        private readonly IFeedBackGenerator _feedback;

        public GameService(
            IWordRepository wordRepo,
            IGuessValidator validator,
            IFeedBackGenerator feedback)
        {
            _wordRepo = wordRepo;
            _validator = validator;
            _feedback = feedback;
        }

        public void StartGame()
        {
            bool playAgain = true;

            while (playAgain)
            {
                Game game = new Game(_wordRepo.GetRandomWord());
                HashSet<string> guesses = new HashSet<string>();
                Console.WriteLine("Find the word!");

                for (int i = 1; i <= game.MaxAttempts; i++)
                {
                    Console.WriteLine($"Attempt {i}: ");
                    string guess = Console.ReadLine()?.ToUpper() ?? string.Empty;

                    try{
                        _validator.ValidateGuess(guess);
                    
                        if (guesses.Contains(guess))
                        {
                            Console.WriteLine("Duplicate guess!");
                            i--;
                            continue;
                        }

                        guesses.Add(guess);
                        string feedback = _feedback.GenerateFeedback(game.SecretWord, guess);
                        Attempt attempt = new Attempt(guess, feedback, i.ToString());
                        game.AddAttempt(attempt);
                        PrintFeedback(attempt);

                        if (guess.Equals(game.SecretWord))
                        {
                            PrintComment(i);
                            GetScore(game.Attempts.Count - 1);

                            break;
                        }
                        if (i == game.MaxAttempts)
                        {
                            Console.WriteLine($"word was: {game.SecretWord}");
                            PrintComment(i);
                            GetScore(game.Attempts.Count);

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

                Console.WriteLine("Game ended");
                Console.WriteLine("1.View current game history");
                Console.WriteLine("2.Play again");
                Console.WriteLine("3.Exit");

                string option = Console.ReadLine() ?? string.Empty;

                if (option == "1")
                {
                    PrintGameHistory(game);

                    Console.WriteLine("Play again? yes or no");

                    playAgain =
                        Console.ReadLine()?.ToLower() == "yes";
                }
                else if (option == "2")
                {
                    playAgain = true;
                }
                else
                {
                    playAgain = false;
                }
            }
        }

        private void PrintGameHistory(Game game)
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

        private void PrintFeedback(Attempt attempt)
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

        private void GetScore(int attempts)
        {
            Console.WriteLine($"Your score is : {100 - (attempts * 10)}");
        }

        private void PrintComment(int attempt)
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

            Console.WriteLine(comments[attempt - 1]);
        }
    }
}