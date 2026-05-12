using WordGame.Interfaces;
using WordGame.Models;
using WordGame.Repositories;
using WordGame.Services;
using WordGame.Exceptions;
using System;

namespace WordGame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IWordRepository wordRepo = new WordRepository();
                IUserService userService = new UserService();
                IGameService gameService = new GameService(wordRepo, userService);

                bool Start = true;

                while (Start)
                {
                    Console.WriteLine("\n== Wordle App ==");
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register");
                    Console.WriteLine("3. Exit");

                    string choice = Console.ReadLine() ?? string.Empty;

                    switch (choice)
                    {
                        case "1":
                            HandleLogin(userService, gameService);
                            break;

                        case "2":
                            HandleRegistration(userService);
                            break;

                        case "3":
                            Start = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nERROR {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void HandleLogin(IUserService userService, IGameService gameService)
        {
            try
            {
                Console.WriteLine("enter the email");
                string email = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("enter the password");
                string password = Console.ReadLine() ?? string.Empty;

                if (userService.Login(email, password))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("==== Login successful! ==");
                    Console.ResetColor();
                    gameService.StartGame();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid email or password.");
                    Console.ResetColor();
                }
            }
            catch (DatabaseException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Login Failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void HandleRegistration(IUserService userService)
        {
            try
            {
                Console.WriteLine("=== User Registration ===");
                Console.WriteLine("enter the name");
                string name = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("enter the email");
                string email = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("enter the password");
                string password = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("confirm password");
                string confirmPassword = Console.ReadLine() ?? string.Empty;

                if (password != confirmPassword)
                {
                    Console.WriteLine("Password doesn't match with confirm password");
                    return;
                }

                if (!GameHelper.IsValidEmail(email))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid email format (e.g., user@example.com)");
                    Console.ResetColor();
                    return;
                }

                User user = userService.CreateUser(name, email, password);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("user created successfully : " + user);
                Console.ResetColor();
            }
            catch (DatabaseException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Registration Failed: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}