using System;
using System.Linq;
using BLLibrary.Interfaces;
using ModelLibrary.Models;
using ModelLibrary.DTOs;
using FELibrary;
using FELibrary.Members;

namespace FELibrary.Admin
{
    public class AdminMenu
    {
        private readonly IMembershipService _membershipService;
        private readonly IBookService _bookService;
        private readonly IReportService _reportService;
        private readonly MemberMenu _memberMenu;
        private readonly ICategoryService _categoryService;

        public AdminMenu(IMembershipService membershipService, IBookService bookService, IReportService reportService, MemberMenu memberMenu, ICategoryService categoryService)
        {
            _membershipService = membershipService;
            _bookService = bookService;
            _reportService = reportService;
            _memberMenu = memberMenu;
            _categoryService = categoryService;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Admin Menu ===");
                Console.WriteLine("1. Membership Management");
                Console.WriteLine("2. Book Management");
                Console.WriteLine("3. Reports");
                Console.WriteLine("4. Member Management");
                Console.WriteLine("5. Category Management");
                Console.WriteLine("0. Logout");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ManageMemberships();
                        break;
                    case "2":
                        ManageBooks();
                        break;
                    case "3":
                        ViewReports();
                        break;
                    case "4":
                        _memberMenu.Show();
                        break;
                    case "5":
                        ManageCategories();
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

        private void ManageBooks()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Book Management ===");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Add Copies to Book");
                Console.WriteLine("3. View All Books");
                Console.WriteLine("4. View a Book's copies");
                Console.WriteLine("5. Mark Copy as Damaged");
                Console.WriteLine("6. Mark Copy as Availble");
                Console.WriteLine("7. View Damage History");
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        AddCopies();
                        break;
                    case "3":
                        ViewAllBooks();
                        break;
                    case "4":
                        GetBookCopies();
                        break;
                    case "5":
                        MarkDamaged();
                        break;
                    case "6":
                        MarkCopyAsAvailable();
                        break;
                    case "7":
                        ViewDamageHistory();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddBook()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Book ---");
            Console.Write("Title: ");
            string? title = Console.ReadLine();
            Console.Write("Author: ");
            string? author = Console.ReadLine();
            Console.Write("ISBN: ");
            string? isbn = Console.ReadLine();
            Console.Write("Published Year: ");
            int.TryParse(Console.ReadLine(), out int year);
            Console.Write("Category ID: ");
            int.TryParse(Console.ReadLine(), out int categoryId);

            try
            {
                var book = new Book
                {
                    Title = title ?? "",
                    Author = author ?? "",
                    ISBN = isbn ?? "",
                    PublishedYear = year,
                    CategoryId = categoryId
                };
                _bookService.AddBook(book);
                Console.WriteLine("Book added successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void AddCopies()
        {
            Console.Clear();
            Console.WriteLine("--- Add Book Copies ---");
            Console.Write("Enter Book ID: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.Write("Enter number of copies to add: ");
                if (int.TryParse(Console.ReadLine(), out int copies))
                {
                    try
                    {
                        _bookService.AddBookCopies(bookId, copies);
                        Console.WriteLine($"{copies} copies added successfully.");
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

        private void ViewAllBooks()
        {
            Console.Clear();
            Console.WriteLine("--- View All Books ---");
            try
            {
                var books = _bookService.GetAllBooks();
                foreach (var b in books)
                {
                    Console.WriteLine(b);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void GetBookCopies()
        {
            Console.Clear();
            Console.WriteLine("Enter the Book id to view its copies");
            int bookId;
            while(!int.TryParse(Console.ReadLine(), out bookId))
            {
                Console.WriteLine("id should be int");
            }
            try
            {
                var bookCopies = _bookService.GetBookCopies(bookId);
                Console.WriteLine(bookCopies.Count());
                foreach(var copy in bookCopies)
                {
                    Console.WriteLine(copy);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void MarkDamaged()
        {
            Console.Clear();
            Console.WriteLine("--- Mark Book Copy as Damaged ---");
            Console.Write("Enter Book Copy ID: ");
            if (int.TryParse(Console.ReadLine(), out int copyId))
            {
                Console.Write("Enter User ID to penalize: ");
                if (int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.Write("Enter Damage Description: ");
                    string? description = Console.ReadLine();
                    Console.Write("Enter Severity (0: LOW, 1: MEDIUM, 2: HIGH): ");
                    if (Enum.TryParse(Console.ReadLine(), out DamageSeverity severity))
                    {
                        try
                        {
                            _bookService.MarkCopyAsDamaged(copyId, userId, description ?? "", severity);
                            Console.WriteLine("Book copy marked as damaged, and fine generated successfully.");
                        }
                        catch (Exception ex)
                        {
                            ConsoleHelper.HandleException(ex);
                        }
                    }
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void MarkCopyAsAvailable()
        {
            Console.Clear();
            Console.WriteLine("--- Mark Book Copy as Availblae ---");
            Console.Write("Enter Book Copy ID: ");
            if (int.TryParse(Console.ReadLine(), out int copyId))
            {
                try
                {
                    _bookService.MarkCopyAsAvailable(copyId);
                    Console.WriteLine("Book status changed successfully");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewDamageHistory()
        {
            Console.Clear();
            Console.WriteLine("--- View Damage History ---");
            Console.Write("Enter Book Copy ID: ");
            if (int.TryParse(Console.ReadLine(), out int copyId))
            {
                try
                {
                    var history = _bookService.GetDamageHistory(copyId);
                    foreach (var h in history)
                    {
                        Console.WriteLine(h);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewReports()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Reports ===");
                Console.WriteLine("1. Books currently borrowed");
                Console.WriteLine("2. Overdue borrowed books");
                Console.WriteLine("3. Members with pending fines");
                Console.WriteLine("4. Most borrowed books");
                Console.WriteLine("5. Available books by category");
                Console.WriteLine("6. Member borrowing history");
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowCurrentlyBorrowed();
                        break;
                    case "2":
                        ShowOverdueBooks();
                        break;
                    case "3":
                        ShowMembersWithPendingFines();
                        break;
                    case "4":
                        ShowMostBorrowedBooks();
                        break;
                    case "5":
                        ShowAvailableBooksByCategory();
                        break;
                    case "6":
                        ShowMemberHistory();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowCurrentlyBorrowed()
        {
            Console.Clear();
            Console.WriteLine("--- Currently Borrowed Books ---");
            try
            {
                var borrowings = _reportService.GetCurrentlyBorrowedBooks();
                if (!borrowings.Any())
                {
                    Console.WriteLine("No books are currently borrowed.");
                }
                else
                {
                    foreach (var b in borrowings)
                    {
                        var memberName = b.Member != null ? b.Member.Name : $"Member #{b.MemberId}";
                        var bookTitle = (b.BookCopy != null && b.BookCopy.Book != null) ? b.BookCopy.Book.Title : $"Copy #{b.CopyId}";
                        Console.WriteLine($"Borrowing ID: {b.BorrowingId}");
                        Console.WriteLine($"Member Name: {memberName}");
                        Console.WriteLine($"Book Title: {bookTitle}");
                        Console.WriteLine($"Borrow Date: {b.BorrowDate:yyyy-MM-dd}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ShowOverdueBooks()
        {
            Console.Clear();
            Console.WriteLine("--- Overdue Books ---");
            try
            {
                var borrowings = _reportService.GetOverdueBooks();
                if (!borrowings.Any())
                {
                    Console.WriteLine("No books are currently overdue.");
                }
                else
                {
                    foreach (var b in borrowings)
                    {
                        var memberName = b.Member != null ? b.Member.Name : $"Member #{b.MemberId}";
                        var bookTitle = (b.BookCopy != null && b.BookCopy.Book != null) ? b.BookCopy.Book.Title : $"Copy #{b.CopyId}";
                        Console.WriteLine($"Borrowing ID: {b.BorrowingId}");
                        Console.WriteLine($"Member Name: {memberName}");
                        Console.WriteLine($"Book Title: {bookTitle}");
                        Console.WriteLine($"Due Date: {b.DueDate:yyyy-MM-dd}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        private void ShowMembersWithPendingFines()
        {
            Console.Clear();
            Console.WriteLine("--- Members With Pending Fines ---");
            try
            {
                var members = _reportService.GetMembersWithPendingFines();
                if (!members.Any())
                {
                    Console.WriteLine("No members have pending fines.");
                }
                else
                {
                    foreach (var m in members)
                    {
                        Console.WriteLine($"Member ID: {m.MemberId}");
                        Console.WriteLine($"Name: {m.MemberName}");
                        Console.WriteLine($"Email: {m.MemberEmail}");
                        Console.WriteLine($"Total Fine: ${m.TotalPendingFine:F2}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ShowMostBorrowedBooks()
        {
            Console.Clear();
            Console.WriteLine("--- Most Borrowed Books ---");
            try
            {
                var books = _reportService.GetMostBorrowedBooks();
                if (!books.Any())
                {
                    Console.WriteLine("No books have been borrowed yet.");
                }
                else
                {
                    foreach (var b in books)
                    {
                        Console.WriteLine($"Book ID: {b.BookId}");
                        Console.WriteLine($"Title: {b.Title}");
                        Console.WriteLine($"Author: {b.Author}");
                        Console.WriteLine($"Times Borrowed: {b.BorrowCount}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ShowAvailableBooksByCategory()
        {
            Console.Clear();
            Console.WriteLine("--- Available Books By Category ---");
            try
            {
                var categories = _reportService.GetAvailableBooksByCategory();
                if (!categories.Any())
                {
                    Console.WriteLine("No categories found.");
                }
                else
                {
                    foreach (var c in categories)
                    {
                        Console.WriteLine($"Category ID: {c.CategoryId}");
                        Console.WriteLine($"Category Name: {c.CategoryName}");
                        Console.WriteLine($"Available Copies: {c.AvailableCopiesCount}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ShowMemberHistory()
        {
            Console.Clear();
            Console.WriteLine("--- Member Borrowing History ---");
            Console.Write("Enter Member ID: ");
            if (int.TryParse(Console.ReadLine(), out int memberId))
            {
                try
                {
                    var history = _reportService.GetMemberBorrowingHistory(memberId);
                    if (!history.Any())
                    {
                        Console.WriteLine($"No borrowing history found for Member {memberId}.");
                    }
                    else
                    {
                        foreach (var h in history)
                        {
                            Console.WriteLine(h);
                            Console.WriteLine("-----------------------------------");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ManageMemberships()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Membership Management ===");
                Console.WriteLine("1. Create Membership");
                Console.WriteLine("2. View All Memberships");
                Console.WriteLine("3. Update Membership");
                Console.WriteLine("4. Deactivate Membership");
                Console.WriteLine("5. Activate Membership");

                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateMembership();
                        break;
                    case "2":
                        ViewAllMemberships();
                        break;
                    case "3":
                        UpdateMembership();
                        break;
                    case "4":
                        DeactivateMembership();
                        break;
                    case "5":
                        ActivateMembership();
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

        private void CreateMembership()
        {
            Console.Clear();
            Console.WriteLine("--- Create Membership ---");
            Console.Write("Name: ");
            string? name = Console.ReadLine();
            Console.Write("Price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.Write("Borrow Limit: ");
                if (int.TryParse(Console.ReadLine(), out int borrowLimit))
                {
                    Console.Write("Borrow Days Limit: ");
                    if (int.TryParse(Console.ReadLine(), out int borrowDaysLimit))
                    {
                        try
                        {
                            _membershipService.CreateMembership(name ?? "", price, borrowLimit, borrowDaysLimit);
                            Console.WriteLine("Membership created successfully.");
                        }
                        catch (Exception ex)
                        {
                            ConsoleHelper.HandleException(ex);
                        }
                    }
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllMemberships()
        {
            Console.Clear();
            Console.WriteLine("--- View All Memberships ---");
            try
            {
                var memberships = _membershipService.GetAllMemberships();
                foreach (var m in memberships)
                {
                    Console.WriteLine(m);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateMembership()
        {
            Console.Clear();
            Console.WriteLine("--- Update Membership ---");
            Console.Write("Enter Membership ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("New Name (leave blank to keep current): ");
                string? name = Console.ReadLine();
                Console.Write("New Price (-1 to keep current): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.Write("New Borrow Limit (-1 to keep current): ");
                    if (int.TryParse(Console.ReadLine(), out int borrowLimit))
                    {
                        Console.Write("New Borrow Days Limit (-1 to keep current): ");
                        if (int.TryParse(Console.ReadLine(), out int borrowDaysLimit))
                        {
                            try
                            {
                                _membershipService.UpdateMembership(id, name ?? "", price, borrowLimit, borrowDaysLimit);
                                Console.WriteLine("Membership updated successfully.");
                            }
                            catch (Exception ex)
                            {
                                ConsoleHelper.HandleException(ex);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
          private void ActivateMembership()
        {
            Console.Clear();
            Console.WriteLine("--- Activate Membership ---");
            Console.Write("Enter Membership ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    _membershipService.ActivateMembership(id);
                    Console.WriteLine("Membership Activated successfully.");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        private void DeactivateMembership()
        {
            Console.Clear();
            Console.WriteLine("--- Deactivate Membership ---");
            Console.Write("Enter Membership ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    _membershipService.DeactivateMembership(id);
                    Console.WriteLine("Membership Deactived successfully.");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ManageCategories()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Category Management ===");
                Console.WriteLine("1. Add Category");
                Console.WriteLine("2. View All Categories");
                Console.WriteLine("3. Update Category Name");
                Console.WriteLine("4. Delete Category");
                Console.WriteLine("0. Back to Admin Menu");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCategory();
                        break;
                    case "2":
                        ViewAllCategories();
                        break;
                    case "3":
                        UpdateCategory();
                        break;
                    case "4":
                        DeleteCategory();
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

        private void AddCategory()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Category ---");
            Console.Write("Category Name: ");
            string? name = Console.ReadLine();

            try
            {
                var category = new BookCategory
                {
                    CategoryName = name ?? ""
                };
                _categoryService.AddCategory(category);
                Console.WriteLine("Category added successfully.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllCategories()
        {
            Console.Clear();
            Console.WriteLine("--- View All Categories ---");
            try
            {
                var categories = _categoryService.GetAllCategories();
                foreach (var c in categories)
                {
                    Console.WriteLine(c);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.HandleException(ex);
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateCategory()
        {
            Console.Clear();
            Console.WriteLine("--- Update Category ---");
            Console.Write("Enter Category ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Enter New Category Name: ");
                string? newName = Console.ReadLine();

                try
                {
                    _categoryService.UpdateCategory(id, newName ?? "");
                    Console.WriteLine("Category updated successfully.");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid Category ID.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Category ---");
            Console.Write("Enter Category ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    _categoryService.DeleteCategory(id);
                    Console.WriteLine("Category deleted successfully.");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.HandleException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid Category ID.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
