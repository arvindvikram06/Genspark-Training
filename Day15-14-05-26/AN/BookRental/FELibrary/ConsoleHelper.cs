using System;
using BLLibrary.Exceptions;

namespace FELibrary
{
    public static class ConsoleHelper
    {
        public static void HandleException(Exception ex)
        {
            if (ex is DataNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[Not Found] {ex.Message}");
            }
            else if (ex is ValidationException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[Validation Error] {ex.Message}");
            }
            else if (ex is ServiceException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Service Error] {ex.Message}");
                // if (ex.InnerException != null)
                // {
                //     Console.WriteLine($"Details: {ex.InnerException.Message}");
                // }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\n[System Error] An unexpected error occurred: {ex.Message}");
            }
            Console.ResetColor();
        }
    }
}
