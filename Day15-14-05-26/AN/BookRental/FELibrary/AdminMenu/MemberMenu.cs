using BLLibrary.Interfaces;
using ModelLibrary.Models;
using BLLibrary.Exceptions;

namespace FELibrary.Members
{
    public class MemberMenu
    {
        private readonly IMemberService _memberService;

        public MemberMenu(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== User/Member Management Menu ===");
                Console.WriteLine("1. Find Member by ID");
                Console.WriteLine("2. Find Member by Email or Phone");
                Console.WriteLine("3. GetAllMembers");
                Console.WriteLine("4. Deactivate Member");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        FindMemberById();
                        break;
                    case "2":
                        FindMemberByEmailOrPhone();
                        break;
                    case "3":
                        GetAll();
                        break;
                    case "4":
                        DeleteMember();
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

        private void FindMemberById()
        {
            Console.Clear();
            Console.WriteLine("--- Find Member by ID ---");
            Console.Write("Enter Member ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    var member = _memberService.GetMemberById(id);
                    Console.WriteLine(member);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void FindMemberByEmailOrPhone()
        {
            Console.Clear();
            Console.WriteLine("--- Find Member by Email or Phone ---");
            Console.WriteLine("1. Search by Email");
            Console.WriteLine("2. Search by Phone Number");
            Console.Write("\nSelect option: ");
            string? choice = Console.ReadLine();

            try
            {
                Member member;
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Email: ");
                        string? email = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(email))
                        {
                            Console.WriteLine("Email cannot be empty.");
                            break;
                        }
                        member = _memberService.GetMemberByEmail(email);
                        Console.WriteLine(member);
                        break;

                    case "2":
                        Console.Write("Enter Phone Number: ");
                        string? phone = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(phone))
                        {
                            Console.WriteLine("Phone Number cannot be empty.");
                            break;
                        }
                        member = _memberService.GetMemberByPhoneNumber(phone);
                        Console.WriteLine(member);
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void GetAll()
        {
            Console.Clear();
            Console.WriteLine("--- Members List ---");
            try
            {
                var members = _memberService.GetAllMembers();

                foreach(var member in members){
                    Console.WriteLine(member);
                }
            }catch(Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

        }

        private void DeleteMember()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Member ---");
            Console.Write("Enter Member ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write($"Are you sure you want to deactivate member {id}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    try
                    {
                        _memberService.DeactivateMember(id);
                        Console.WriteLine("\nMember deactivated successfully!");
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.HandleException(ex);
                    }
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
