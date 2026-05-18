using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;
using DALLibrary.Configurations;
using DALLibrary.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DALLibrary.Contexts
{
    public class BookRentalContext : DbContext
    {
        public DbSet<Member> Members => Set<Member>();

        public DbSet<Membership> Memberships => Set<Membership>();

        public DbSet<MembershipPayment> MembershipPayments => Set<MembershipPayment>();

        public DbSet<Book> Books => Set<Book>();

        public DbSet<BookCategory> BookCategories => Set<BookCategory>();

        public DbSet<BookCopy> BookCopies => Set<BookCopy>();

        public DbSet<BookDamageHistory> BookDamageHistories => Set<BookDamageHistory>();

        public DbSet<Borrowing> Borrowings => Set<Borrowing>();

        public DbSet<Fine> Fines => Set<Fine>();

        public DbSet<FinePayment> FinePayments => Set<FinePayment>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=BookRental;Username=postgres;Password=Arvind"
            );
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MemberConfiguration());
            modelBuilder.ApplyConfiguration(new MembershipConfiguration());
            modelBuilder.ApplyConfiguration(new MembershipPaymentConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new BookCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BookCopyConfiguration());
            modelBuilder.ApplyConfiguration(new BookDamageHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new BorrowingConfiguration());
            modelBuilder.ApplyConfiguration(new FineConfiguration());
            modelBuilder.ApplyConfiguration(new FinePaymentConfiguration());

            // Add Seed Data
            modelBuilder.Seed();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Database operation failed.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Unexpected db error occurred.", ex);
            }
        }
    }
}