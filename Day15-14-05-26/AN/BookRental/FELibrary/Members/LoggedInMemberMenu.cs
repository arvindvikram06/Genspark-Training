using System;
using BLLibrary.Interfaces;
using ModelLibrary.Models;

namespace FELibrary.Members
{
    public class LoggedInMemberMenu
    {
        private readonly IMemberService _memberService;
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IFineService _fineService;
        private readonly IMembershipService _membershipService;

        public LoggedInMemberMenu(
            IMemberService memberService,
            IBorrowingService borrowingService,
            IBookService bookService,
            IFineService fineService,
            IMembershipService membershipService)
        {
            _memberService = memberService;
            _borrowingService = borrowingService;
            _bookService = bookService;
            _fineService = fineService;
            _membershipService = membershipService;
        }

        public void Show(int loggedInUserId)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Member Dashboard ===");
                Console.WriteLine("1. View Profile");
                Console.WriteLine("2. Manage Membership");
                Console.WriteLine("3. Search Books (Title/Author)");
                Console.WriteLine("4. Search Books by Category");
                Console.WriteLine("5. View Available Books");
                Console.WriteLine("6. Borrow Book");
                Console.WriteLine("7. View Borrowed Books");
                Console.WriteLine("8. Return a Book");
                Console.WriteLine("9. Manage Fines");
                Console.WriteLine("0. Logout");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewProfile(loggedInUserId);
                        break;
                    case "2":
                        ManageMembership(loggedInUserId);
                        break;
                    case "3":
                        SearchBooks();
                        break;
                    case "4":
                        SearchBooksByCategory();
                        break;
                    case "5":
                        ViewAvailableBooks();
                        break;
                    case "6":
                        BorrowBook(loggedInUserId);
                        break;
                    case "7":
                        ViewBorrowedBooks(loggedInUserId);
                        break;
                    case "8":
                        ReturnBook(loggedInUserId);
                        break;
                    case "9":
                        ManageFines(loggedInUserId);
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

        private void ViewProfile(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- My Profile ---");
            try
            {
                var member = _memberService.GetMemberById(userId);
                Console.WriteLine($"ID: {member.MemberId}");
                Console.WriteLine($"Name: {member.Name}");
                Console.WriteLine($"Email: {member.Email}");
                Console.WriteLine($"Phone: {member.PhoneNumber}");
                Console.WriteLine($"Membership Status: {member.MembershipStatus}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ManageMembership(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- Manage Membership ---");
            Console.WriteLine("1. View My Membership");
            Console.WriteLine("2. Purchase/Upgrade Membership");
            Console.WriteLine("0. Back");
            Console.Write("\nSelect an option: ");
            string? choice = Console.ReadLine();
            
            if (choice == "1")
            {
                try
                {
                    var member = _memberService.GetMemberById(userId);
                    if (member.Membership != null)
                    {
                        Console.WriteLine($"Membership: {member.Membership.MembershipName}");
                        Console.WriteLine($"Borrow Limit: {member.Membership.BorrowLimit}");
                        Console.WriteLine($"Days Limit: {member.Membership.BorrowDaysLimit}");
                        Console.WriteLine($"Start Date: {member.MembershipStartDate}");
                        Console.WriteLine($"End Date: {member.MembershipEndDate}");
                    }
                    else
                    {
                        Console.WriteLine("No active membership.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else if (choice == "2")
            {
                try
                {
                    var memberships = _membershipService.GetAllMemberships().Where(m => m.IsActive != false);
                    Console.WriteLine("\n--- Available Memberships ---");
                    foreach (var m in memberships)
                    {
                        Console.WriteLine($"ID: {m.MembershipId} | Name: {m.MembershipName} | Price: ₹{m.MembershipPrice} | Limit: {m.BorrowLimit} books | Duration: {m.BorrowDaysLimit} days limit per book");
                    }

                    Console.Write("\nEnter Membership ID to purchase/upgrade: ");
                    if (int.TryParse(Console.ReadLine(), out int membershipId))
                    {
                        _membershipService.PurchaseMembership(userId, membershipId);
                        Console.WriteLine("Membership purchased successfully and payment recorded!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchBooks()
        {
            Console.Clear();
            Console.WriteLine("--- Search Books ---");
            Console.Write("Enter Title or Author to search: ");
            string? query = Console.ReadLine();
            try
            {
                var books = _bookService.SearchBooks(query ?? "");
                foreach (var book in books)
                {
                    Console.WriteLine(book);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchBooksByCategory()
        {
            Console.Clear();
            Console.WriteLine("--- Search Books by Category ---");
            Console.Write("Enter Category Name to search: ");
            string? query = Console.ReadLine();
            try
            {
                var books = _bookService.SearchBooksByCategory(query ?? "");
                if (!books.Any())
                {
                    Console.WriteLine("No books found matching this category.");
                }
                else
                {
                    foreach (var book in books)
                    {
                        Console.WriteLine(book);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewAvailableBooks()
        {
            Console.Clear();
            Console.WriteLine("--- Available Books ---");
            try
            {
                var books = _bookService.GetAvailableBooks();
                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.BookId} | Title: {book.Title} | Author: {book.Author}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void BorrowBook(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- Borrow a Book ---");
            Console.Write("Enter Book ID to borrow: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                try
                {
                    _borrowingService.BorrowBook(userId, bookId);
                    Console.WriteLine("Successfully borrowed the book!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to borrow: {ex.Message}");
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewBorrowedBooks(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- My Borrowed Books ---");
            try
            {
                var borrowings = _borrowingService.GetBorrowedBooksByMember(userId);
                foreach (var b in borrowings)
                {
                    Console.WriteLine(b);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ReturnBook(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- Return a Book ---");
            Console.Write("Enter Copy ID to return: ");
            if (int.TryParse(Console.ReadLine(), out int copyId))
            {
                try
                {
                    _borrowingService.ReturnBook(userId, copyId);
                    Console.WriteLine("Successfully returned the book!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to return: {ex.Message}");
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ManageFines(int userId)
        {
            Console.Clear();
            Console.WriteLine("--- Fine Management ---");
            Console.WriteLine("1. View Pending Fines & Pay");
            Console.WriteLine("2. View Fine History");
            Console.WriteLine("0. Back");
            Console.Write("\nSelect an option: ");
            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                try
                {
                    var fines = _fineService.GetFinesForMember(userId);
                    bool hasPending = false;
                    foreach (var f in fines)
                    {
                        if (f.PaidStatus == FineStatus.PENDING)
                        {
                            Console.WriteLine(f);
                            hasPending = true;
                        }
                    }

                    if (hasPending)
                    {
                        Console.Write("\nEnter Fine ID to pay: ");
                        if (int.TryParse(Console.ReadLine(), out int fineId))
                        {
                            _fineService.PayFine(fineId);
                            Console.WriteLine("Fine paid successfully!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No pending fines!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else if (choice == "2")
            {
                try
                {
                    var fines = _fineService.GetFinesForMember(userId);
                    foreach (var f in fines)
                    {
                        Console.WriteLine(f);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
