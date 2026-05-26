using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BankingApiTest
{
    public class TransactionRepositoryTest
    {
        private IRepository<int, Transaction> _transactionRepository;
        private BankingContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BankingContext>()
                .UseInMemoryDatabase("BankingTestDb")
                .Options;

            _context = new BankingContext(options);

            _context.Database.EnsureDeleted();

            _transactionRepository = new Repository<int, Transaction>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddTransactionPassTest()
        {
            var transaction = new Transaction
            {
                TransactionReferenceNumber = 1001,
                TransactionDate = DateTime.Today,
                FromAccountNumber = "111",
                ToAccountNumber = "222",
                Amount = 1500m,
                Status = "Success"
            };

            var result = await _transactionRepository.Create(transaction);

            Assert.That(result.TransactionReferenceNumber, Is.EqualTo(1001));
        }

        [Test]
        public async Task GetTransactionPassTest()
        {
            var transaction = new Transaction
            {
                TransactionReferenceNumber = 2002,
                TransactionDate = DateTime.Today,
                FromAccountNumber = "111",
                ToAccountNumber = null,
                Amount = 500m,
                Status = "Success"
            };
            var seed = await _transactionRepository.Create(transaction);

            var result = await _transactionRepository.Get(seed.TransactionReferenceNumber);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(500m));
            Assert.That(result.FromAccountNumber, Is.EqualTo("111"));
        }
        [Test]
        public async Task DeleteTransactionExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await _transactionRepository.Delete(9999));
        }
    }
}