using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;
using System;

namespace DALLibrary.Contexts
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<BookCategory>().HasData(
                new BookCategory { CategoryId = 1, CategoryName = "Fantasy" },
                new BookCategory { CategoryId = 2, CategoryName = "Science Fiction" },
                new BookCategory { CategoryId = 3, CategoryName = "Mystery" },
                new BookCategory { CategoryId = 4, CategoryName = "Non-Fiction" }
            );

            // Seed Memberships
            modelBuilder.Entity<Membership>().HasData(
                new Membership { MembershipId = 1, MembershipName = "Basic", MembershipPrice = 500m, BorrowLimit = 2, BorrowDaysLimit = 7 },
                new Membership { MembershipId = 2, MembershipName = "Premium", MembershipPrice = 1500m, BorrowLimit = 5, BorrowDaysLimit = 14 },
                new Membership { MembershipId = 3, MembershipName = "Elite", MembershipPrice = 3000m, BorrowLimit = 10, BorrowDaysLimit = 30 }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", ISBN = "978-0439708180", PublishedYear = 1997, CategoryId = 1 },
                new Book { BookId = 2, Title = "Dune", Author = "Frank Herbert", ISBN = "978-0441172719", PublishedYear = 1965, CategoryId = 2 },
                new Book { BookId = 3, Title = "Sherlock Holmes", Author = "Arthur Conan Doyle", ISBN = "978-0553212419", PublishedYear = 1892, CategoryId = 3 },
                new Book { BookId = 4, Title = "Atomic Habits", Author = "James Clear", ISBN = "978-0735211292", PublishedYear = 2018, CategoryId = 4 }
            );

            // Seed Book Copies
            modelBuilder.Entity<BookCopy>().HasData(
                new BookCopy { CopyId = 1, BookId = 1, CopyNumber = "HP-001", CopyStatus = BookCopyStatus.AVAILABLE, AddedDate = new DateTime(2023, 1, 1) },
                new BookCopy { CopyId = 2, BookId = 1, CopyNumber = "HP-002", CopyStatus = BookCopyStatus.AVAILABLE, AddedDate = new DateTime(2023, 1, 1) },
                new BookCopy { CopyId = 3, BookId = 2, CopyNumber = "DU-001", CopyStatus = BookCopyStatus.AVAILABLE, AddedDate = new DateTime(2023, 2, 1) },
                new BookCopy { CopyId = 4, BookId = 3, CopyNumber = "SH-001", CopyStatus = BookCopyStatus.AVAILABLE, AddedDate = new DateTime(2023, 3, 1) },
                new BookCopy { CopyId = 5, BookId = 4, CopyNumber = "AH-001", CopyStatus = BookCopyStatus.AVAILABLE, AddedDate = new DateTime(2023, 4, 1) }
            );

            // Seed Members
            modelBuilder.Entity<Member>().HasData(
                new Member 
                { 
                    MemberId = 1, 
                    Name = "Alice Smith", 
                    Email = "alice@example.com", 
                    PhoneNumber = "1234567890", 
                    MembershipId = 2, // Premium
                    MembershipStatus = MembershipStatus.ACTIVE,
                    MembershipStartDate = new DateTime(2023, 1, 1),
                    MembershipEndDate = new DateTime(2024, 1, 1),
                    IsActive = true
                },
                new Member 
                { 
                    MemberId = 2, 
                    Name = "Bob Jones", 
                    Email = "bob@example.com", 
                    PhoneNumber = "0987654321", 
                    MembershipId = null,
                    MembershipStatus = MembershipStatus.NOT_PURCHASED,
                    MembershipStartDate = null,
                    MembershipEndDate = null,
                    IsActive = true
                }
            );
        }
    }
}
