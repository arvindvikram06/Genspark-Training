using ModelLibrary.Models;
using ModelLibrary.DTOs;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using System.Collections.Generic;

namespace BLLibrary.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public IEnumerable<Borrowing> GetCurrentlyBorrowedBooks()
        {
            return _reportRepository.GetCurrentlyBorrowedBooks();
        }

        public IEnumerable<Borrowing> GetOverdueBooks()
        {
            return _reportRepository.GetOverdueBooks();
        }

        public IEnumerable<MostBorrowedBookDto> GetMostBorrowedBooks()
        {
            return _reportRepository.GetMostBorrowedBooks();
        }

        public IEnumerable<AvailableBooksByCategoryDto> GetAvailableBooksByCategory()
        {
            return _reportRepository.GetAvailableBooksByCategory();
        }

        public IEnumerable<Borrowing> GetMemberBorrowingHistory(int memberId)
        {
            return _reportRepository.GetMemberBorrowingHistory(memberId);
        }

        public IEnumerable<MemberPendingFineDto> GetMembersWithPendingFines()
        {
            return _reportRepository.GetMembersWithPendingFines();
        }
    }
}
