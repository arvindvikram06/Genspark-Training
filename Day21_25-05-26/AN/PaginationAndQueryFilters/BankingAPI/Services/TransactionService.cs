using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using BankingAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankingAPI.Services
{
    public class TransactionService : ITransact
    {
        private readonly BankingContext _context;

        public TransactionService(BankingContext context)
        {
            _context = context;
        }

        public TransactionResponse Deposit(DepositRequest request)
        {
            var account = _context.Accounts
                .SingleOrDefault(a => a.AccountNumber == request.ToAccountNumber);

            if (account == null)
                throw new ArgumentException(
                    "Destination account not found: " + request.ToAccountNumber);

            using var dbTxn = _context.Database.BeginTransaction();

            try
            {
                // Update balance
                account.Balance += (float)request.Amount;

                _context.Accounts.Update(account);
                _context.SaveChanges();

                // Create transaction record
                var tx = new Transaction
                {
                    TransactionDate = DateTime.Now,

                    FromAccountNumber = null,

                    ToAccountNumber = account.AccountNumber,

                    Amount = request.Amount,

                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);

                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                dbTxn.Rollback();
                throw;
            }
        }

        public TransactionResponse Withdraw(WithdrawRequest request)
        {
            var account = _context.Accounts
                .SingleOrDefault(a => a.AccountNumber == request.FromAccountNumber);

            if (account == null)
                throw new ArgumentException(
                    "Source account not found: " + request.FromAccountNumber);

            var amt = (float)request.Amount;

            if (account.Balance < amt)
                throw new InvalidOperationException("Insufficient funds");

            using var dbTxn = _context.Database.BeginTransaction();

            try
            {
                // Update balance
                account.Balance -= amt;

                _context.Accounts.Update(account);
                _context.SaveChanges();

                // Create transaction record
                var tx = new Transaction
                {
                    TransactionDate = DateTime.Now,

                    FromAccountNumber = account.AccountNumber,

                    ToAccountNumber = null,

                    Amount = request.Amount,

                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);

                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                dbTxn.Rollback();
                throw;
            }
        }

        public TransactionResponse Transfer(TransferRequest request)
        {
            if (request.FromAccountNumber == request.ToAccountNumber)
                throw new ArgumentException(
                    "From and To account numbers must differ.");

            var from = _context.Accounts
                .SingleOrDefault(a => a.AccountNumber == request.FromAccountNumber);

            var to = _context.Accounts
                .SingleOrDefault(a => a.AccountNumber == request.ToAccountNumber);

            if (from == null)
                throw new ArgumentException(
                    "Source account not found: " + request.FromAccountNumber);

            if (to == null)
                throw new ArgumentException(
                    "Destination account not found: " + request.ToAccountNumber);

            var amt = (float)request.Amount;

            if (from.Balance < amt)
                throw new InvalidOperationException(
                    "Insufficient funds in source account");

            using var dbTxn = _context.Database.BeginTransaction();

            try
            {
                // Update balances
                from.Balance -= amt;
                to.Balance += amt;

                _context.Accounts.Update(from);
                _context.Accounts.Update(to);

                _context.SaveChanges();

                // Minimum balance validation
                if (from.Balance < 1000f)
                {
                    dbTxn.Rollback();

                    throw new InvalidOperationException(
                        "Transfer would reduce source balance below required minimum of 1000.");
                }

                // Create transaction record
                var tx = new Transaction
                {
                    TransactionDate = DateTime.Now,

                    FromAccountNumber = from.AccountNumber,

                    ToAccountNumber = to.AccountNumber,

                    Amount = request.Amount,

                    Status = "Success"
                };

                var created = _context.Transactions.Add(tx);

                _context.SaveChanges();

                dbTxn.Commit();

                return Map(created.Entity);
            }
            catch
            {
                    dbTxn.Rollback();
                    throw;
            }
        }

        public TransactionResponse? GetTransactionByReference(
            int referenceNumber)
        {
            var tx = _context.Transactions.Find(referenceNumber);

            if (tx == null)
                return null;

            return Map(tx);
        }


        public PagedResult<TransactionResponse> GetTransactions(TransactionQueryParams query)
        {
            var transactions = _context.Transactions.AsQueryable();

            

            if (!string.IsNullOrWhiteSpace(query.AccountNumber))
            {
                transactions = transactions.Where(t => 
                t.FromAccountNumber == query.AccountNumber || t.ToAccountNumber == query.AccountNumber);
            }

            if (query.FromDate.HasValue)
            {
                transactions = transactions.Where(t => t.TransactionDate.Date >= query.FromDate.Value.Date);
            }

            if (query.ToDate.HasValue)
            {
                transactions = transactions.Where(t => t.TransactionDate.Date <= query.ToDate.Value.Date);
            }

            if (query.MinAmount.HasValue)
            {
                transactions = transactions.Where(t => t.Amount >= query.MinAmount.Value);
            }

             if (query.MaxAmount.HasValue)
            {
                transactions = transactions.Where(t => t.Amount <= query.MaxAmount.Value);
            }

            transactions = query.SortBy.ToLower()
                switch
            {
                "amount" => query.SortOrder.ToLower() == "asc"
                            ? transactions.OrderBy(t => t.Amount) : transactions.OrderByDescending(t => t.Amount),

                _ => query.SortOrder.ToLower() == "asc"
                            ? transactions.OrderBy(t => t.TransactionDate) : transactions.OrderByDescending(t => t.TransactionDate)
            };


            var totalCount = transactions.Count();


            var val = transactions.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList().Select(Map);
            
            return new PagedResult<TransactionResponse>
            {
                Data = val,

            };
        }

        private TransactionResponse Map(Transaction t)
        {
            return new TransactionResponse
            {
                TransactionReferenceNumber =
                    t.TransactionReferenceNumber,

                TransactionDate =
                    t.TransactionDate,

                FromAccountNumber =
                    t.FromAccountNumber ?? string.Empty,

                ToAccountNumber =
                    t.ToAccountNumber ?? string.Empty,

                Amount =
                    t.Amount,

                Status =
                    t.Status
            };
        }
    }
}