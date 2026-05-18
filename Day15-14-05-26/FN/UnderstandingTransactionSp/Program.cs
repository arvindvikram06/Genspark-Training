using Microsoft.EntityFrameworkCore;
using UnderstandingTransactionSp.Contexts;
using UnderstandingTransactionSp.Models;

internal class Program
{
    private readonly AppDbContext _context;

    Program()
    {
        _context = new AppDbContext();
    }

    void TransactWithTransactioninDatabase()
    {
        int fromAccountNo = 5;
        int toAccountNo = 6;

        float amount = 100;
        int tran_id = 7;

        // Fetch accounts from DB
        Account1? fc = _context.Account1s
            .FirstOrDefault(a => a.Aacno == fromAccountNo);

        Account1? tc = _context.Account1s
            .FirstOrDefault(a => a.Aacno == toAccountNo);

        // Null check
        if (fc == null || tc == null)
        {
            Console.WriteLine("Account not found");
            return;
        }

        // Begin transaction
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            // Check balance
            if (fc.Balance < amount)
                throw new Exception("Insufficient balance");

            // Add transaction record
            _context.Database.ExecuteSqlInterpolated(
                $"CALL add_trans({tran_id}, {fc.Aacno}, {tc.Aacno}, {amount})"
            );

            // Deduct sender balance
            _context.Database.ExecuteSqlInterpolated(
                $"CALL update_account({fc.Aacno}, {fc.Balance - amount})"
            );

            // Add receiver balance
            _context.Database.ExecuteSqlInterpolated(
                $"CALL update_account({tc.Aacno}, {tc.Balance + amount})"
            );

            // Commit transaction
            transaction.Commit();

            Console.WriteLine("Transaction successful");
        }
        catch (Exception ex)
        {
            // Rollback on failure
            transaction.Rollback();

            Console.WriteLine($"Transaction failed: {ex.Message}");
        }
    }

    void AddAccountUsingSP()
    {
        Account1 account = new Account1()
        {
            Aacno = 1,
            Balance = 1233.3f
        };

        try
        {
            _context.Database.ExecuteSqlInterpolated(
                $"CALL add_account({account.Aacno}, {account.Balance})"
            );

            Console.WriteLine("Account Created");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create account: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        Program program = new Program();

        program.TransactWithTransactioninDatabase();

        // program.AddAccountUsingSP();
    }
}