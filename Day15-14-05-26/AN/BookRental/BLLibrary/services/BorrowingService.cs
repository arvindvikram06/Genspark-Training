using ModelLibrary.Models;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using BLLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using DALLibrary;
using DALLibrary.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BLLibrary.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        private readonly IFineRepository _fineRepository;
        private readonly BookRentalContext _context;

        public BorrowingService(
            IBorrowingRepository borrowingRepository,
            IMemberRepository memberRepository,
            IBookCopyRepository bookCopyRepository,
            IFineRepository fineRepository,
            BookRentalContext context)
        {
            _borrowingRepository = borrowingRepository;
            _memberRepository = memberRepository;
            _bookCopyRepository = bookCopyRepository;
            _fineRepository = fineRepository;
            _context = context;
        }

        public void BorrowBook(int memberId, int bookId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // check user exist
                var member = _memberRepository.GetById(memberId);
                if (member == null)
                {
                    throw new DataNotFoundException("Member", memberId);
                }
                if (!member.IsActive)
                {
                    throw new ValidationException("Member is inactive.");
                }

                if (member.MembershipStatus != MembershipStatus.ACTIVE || member.Membership == null)
                {
                    throw new ValidationException("Member does not have an active membership.");
                }

                decimal totalUnpaid = 0;

                // get total unpaid fines
                totalUnpaid = _context.Database.SqlQueryRaw<decimal>("SELECT get_total_unpaid_fine({0})", memberId).ToList().FirstOrDefault();

                if (totalUnpaid > 500)
                {
                    throw new ValidationException("Member has unpaid fines exceeding ₹500. Please pay fines to continue borrowing.");
                }

                //Check active borrowing count
                var activeBorrowings = _borrowingRepository.GetBorrowingsByMemberId(memberId).Where(b => b.Status == BorrowingStatus.ACTIVE);
                if (activeBorrowings.Count() >= member.Membership.BorrowLimit)
                {
                    throw new ValidationException("Member has reached the maximum borrow limit.");
                }

                //Prevent borrowing the same book if already active
                var alreadyBorrowed = _context.Borrowings.Any(b => b.MemberId == memberId && b.Status == BorrowingStatus.ACTIVE && b.BookCopy.BookId == bookId);
                if (alreadyBorrowed)
                {
                    throw new ValidationException("You have already borrowed this book and have not returned it yet.");
                }

                //Check book copy availability
                var availableCopy = _bookCopyRepository.GetAvailableCopy(bookId);
                if (availableCopy == null)
                {
                    throw new ServiceException("No available copies for this book.");
                }

                // Create borrowing record
                var borrowing = new Borrowing
                {
                    MemberId = memberId,
                    CopyId = availableCopy.CopyId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(member.Membership.BorrowDaysLimit),
                    Status = BorrowingStatus.ACTIVE
                };

                _borrowingRepository.Add(borrowing);

                //Update book copy status as borrowed
                availableCopy.CopyStatus = BookCopyStatus.BORROWED;
                _bookCopyRepository.UpdateBookCopy(availableCopy);

                _context.SaveChanges();

                //Commit transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void ReturnBook(int memberId, int copyId)
        {
            var borrowing = _borrowingRepository.GetBorrowingsByMemberId(memberId)
                .FirstOrDefault(b => b.CopyId == copyId && b.Status == BorrowingStatus.ACTIVE);

            if (borrowing == null)
            {
                throw new DataNotFoundException("Borrowing", $"Member: {memberId}, Copy: {copyId}");
            }

            borrowing.ReturnedDate = DateTime.Now;
            borrowing.Status = BorrowingStatus.RETURNED;

            var copy = _bookCopyRepository.GetBookCopyById(copyId);
            if (copy != null)
            {
                copy.CopyStatus = BookCopyStatus.AVAILABLE;
                _bookCopyRepository.UpdateBookCopy(copy);
            }

            // fine calculation
            if (borrowing.ReturnedDate > borrowing.DueDate)
            {
                int daysLate = (borrowing.ReturnedDate.Value - borrowing.DueDate).Days;
                var fine = new Fine
                {
                    BorrowingId = borrowing.BorrowingId,
                    FineAmount = daysLate * 10,
                    Reason = FineReason.LATE_RETURN,
                    PaidStatus = FineStatus.PENDING,
                    CreatedAt = DateTime.Now
                };
                _fineRepository.Add(fine);
            }

            _context.SaveChanges();
        }

        public IEnumerable<Borrowing> GetBorrowedBooksByMember(int memberId)
        {
            return _borrowingRepository.GetBorrowingsByMemberId(memberId);
        }
    }
}
