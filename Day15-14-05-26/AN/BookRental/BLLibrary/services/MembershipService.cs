using ModelLibrary.Models;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using DALLibrary.Exceptions;
using BLLibrary.Exceptions;
using System.Collections.Generic;
using DALLibrary.Contexts;
using System;

namespace BLLibrary.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly BookRentalContext _context;

        public MembershipService(IMembershipRepository membershipRepository, BookRentalContext context)
        {
            _membershipRepository = membershipRepository;
            _context = context;
        }

        public void CreateMembership(string name, decimal price, int borrowLimit, int borrowDaysLimit)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name cannot be empty.");
            if (price < 0) throw new ValidationException("Price cannot be negative.");
            if (borrowLimit < 0) throw new ValidationException("Borrow limit cannot be negative.");
            if (borrowDaysLimit < 0) throw new ValidationException("Borrow days limit cannot be negative.");

            var membership = new Membership
            {
                MembershipName = name,
                MembershipPrice = price,
                BorrowLimit = borrowLimit,
                BorrowDaysLimit = borrowDaysLimit
            };

            try
            {
                _membershipRepository.Add(membership);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to create membership.", ex);
            }
        }

        public void UpdateMembership(int id, string name, decimal price, int borrowLimit, int borrowDaysLimit)
        {
            var membership = _membershipRepository.GetById(id);
            if (membership == null) throw new DataNotFoundException("Membership", id);

            if (!string.IsNullOrWhiteSpace(name)) membership.MembershipName = name;
            if (price >= 0) membership.MembershipPrice = price;
            if (borrowLimit >= 0) membership.BorrowLimit = borrowLimit;
            if (borrowDaysLimit >= 0) membership.BorrowDaysLimit = borrowDaysLimit;

            try
            {
                _membershipRepository.Update(membership);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to update membership.", ex);
            }
        }

        public void ActivateMembership(int id)
        {
            var membership = _membershipRepository.GetById(id);
            if (membership == null) throw new DataNotFoundException("Membership", id);

            try
            {
                membership.IsActive = true;
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to activate membership.", ex);
            }
        }

        public void DeactivateMembership(int id)
        {
            var membership = _membershipRepository.GetById(id);
            if (membership == null) throw new DataNotFoundException("Membership", id);

            try
            {
                membership.IsActive = false;
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to delete membership.", ex);
            }
        }

        public Membership GetMembershipById(int id)
        {
            var membership = _membershipRepository.GetById(id);
            if (membership == null) throw new DataNotFoundException("Membership", id);
            return membership;
        }

        public IEnumerable<Membership> GetAllMemberships()
        {
            return _membershipRepository.GetAll();
        }

        public void PurchaseMembership(int memberId, int membershipId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var member = _context.Members.Find(memberId);
                if (member == null)
                {
                    throw new DataNotFoundException("Member", memberId);
                }
                if (!member.IsActive)
                {
                    throw new ValidationException("Member is inactive.");
                }

                var membership = _context.Memberships.Find(membershipId);
                if (membership == null)
                {
                    throw new DataNotFoundException("Membership", membershipId);
                }

                member.MembershipId = membershipId;
                member.MembershipStatus = MembershipStatus.ACTIVE;
                member.MembershipStartDate = DateTime.Now;
                member.MembershipEndDate = DateTime.Now.AddMonths(3);

                _context.Members.Update(member);

                var payment = new MembershipPayment
                {
                    MemberId = memberId,
                    MembershipId = membershipId,
                    PaidAmount = membership.MembershipPrice,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    PaymentDate = DateTime.Now
                };

                _context.MembershipPayments.Add(payment);
                _context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (ex is DataNotFoundException) throw;
                throw new ServiceException("Failed to purchase membership.", ex);
            }
        }
    }
}
