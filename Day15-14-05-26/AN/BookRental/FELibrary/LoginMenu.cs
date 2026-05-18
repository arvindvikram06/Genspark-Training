using System.ComponentModel.DataAnnotations;
using BLLibrary.Interfaces;
using FELibrary.Admin;
using FELibrary.Members;
using ModelLibrary.Models;
using FELibrary;

namespace FELibrary.Menus
{
    public class LoginMenu
    {
        static Member? loggedMember = null;

        private readonly IMemberService _memberService;
        private readonly AdminMenu _adminMenu;
        private readonly LoggedInMemberMenu _loggedInMemberMenu;

        public LoginMenu(IMemberService memberService, AdminMenu adminMenu, LoggedInMemberMenu loggedInMemberMenu)
        {
            _memberService = memberService;
            _adminMenu = adminMenu;
            _loggedInMemberMenu = loggedInMemberMenu;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Login Menu ===");
                Console.WriteLine("1. Admin operation");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Login");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _adminMenu.Show();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        Login();

                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void Register()
        {
        {
            Console.Clear();
            Console.WriteLine("--- Register New Member ---");
            Console.Write("Enter Name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter Email: ");
            string? email = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            string? phone = Console.ReadLine();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone))
            {
                Console.WriteLine("All fields are required.");
                Console.ReadKey();
                return;
            }

            try
            {
                _memberService.RegisterMember(name, email, phone);
                Console.WriteLine("\nmember registered successfully!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }

            Console.WriteLine("\npress any key to continue...");
            Console.ReadKey();
        }
            
        }

        private void Login()
        {
            Console.Clear();
            
            Console.Write("Enter your email: ");
            string? email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email cannot be empty.");
                return;
            }

            try
            {
                loggedMember = _memberService.GetMemberByEmail(email);
                Console.WriteLine("Login Successful");
                _loggedInMemberMenu.Show(loggedMember.MemberId);
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

        }
    }
}
