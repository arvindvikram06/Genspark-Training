using ModelLibrary.Models;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using BLLibrary.Exceptions;
using System.Collections.Generic;
using DALLibrary.Contexts;
using DALLibrary.Exceptions;

namespace BLLibrary.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fineRepository;
        private readonly BookRentalContext _context;

        public FineService(IFineRepository fineRepository, BookRentalContext context)
        {
            _fineRepository = fineRepository;
            _context = context;
        }

        public IEnumerable<Fine> GetFinesForMember(int memberId)
        {
            return _fineRepository.GetFinesByMemberId(memberId);
        }

        public void PayFine(int fineId)
        {
            var fine = _fineRepository.GetById(fineId);
            if (fine == null) throw new DataNotFoundException("Fine", fineId);
            
            if (fine.PaidStatus == FineStatus.PAID)
            {
                throw new ValidationException("Fine is already paid.");
            }

            try
            {
                fine.PaidStatus = FineStatus.PAID;
                _fineRepository.Update(fine);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to pay fine.", ex);
            }
        }
    }
}
