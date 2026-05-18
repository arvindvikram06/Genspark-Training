
using ModelLibrary.Models;
using BLLibrary.Validators;
using BLLibrary.Exceptions;
using DALLibrary.Exceptions;
using DALLibrary;
using BLLibrary.Interfaces;
using DALLibrary.Contexts;

namespace BLLibrary.Services{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly BookRentalContext _context;
        public MemberService(IMemberRepository memberRepository, BookRentalContext context){
            _memberRepository = memberRepository;
            _context = context;
        }
        public void RegisterMember(string name, string email, string phoneNumber)
        {
            MemberValidator.ValidateMemberInput(name, email, phoneNumber);

            Member member = new Member
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };

            try{
                _memberRepository.Add(member);
                _context.SaveChanges();
            }
            catch(RepositoryException ex){
                throw new ServiceException("Unable to register member.", ex);
            }
            
        }

        public void UpdateMember(int memberId, string field, string newValue)
        {
            Member? member = _memberRepository.GetById(memberId);
            if (member == null)
            {
                throw new DataNotFoundException("Member", memberId);
            }

            switch (field.ToLower())
            {
                case "name":
                    MemberValidator.ValidateName(newValue);
                    member.Name = newValue;
                    break;
                case "email":
                    MemberValidator.ValidateEmail(newValue);
                    member.Email = newValue;
                    break;
                case "phone":
                    MemberValidator.ValidatePhoneNumber(newValue);
                    member.PhoneNumber = newValue;
                    break;
                default:
                    throw new ArgumentException("Invalid field name.");
            }
    
            try{
                _memberRepository.Update(member);
                _context.SaveChanges();
            }
            catch(RepositoryException ex){
                throw new ServiceException("Unable to update member.", ex);
            }
        }

        public void DeactivateMember(int memberId)
        {
            Member? member = _memberRepository.GetById(memberId);
            if (member == null)
            {
                throw new DataNotFoundException("Member", memberId);
            }

            try
            {
                member.IsActive = false;
                _memberRepository.Update(member);
                _context.SaveChanges();
            }
            catch(RepositoryException ex)
            {
                throw new ServiceException("Unable to deactivate member.", ex);
            }
        }


        public IEnumerable<Member> GetAllMembers()
        {
            return _memberRepository.GetAll();
        }

        public Member GetMemberById(int memberId)
        {
            Member? member = _memberRepository.GetById(memberId);

            if (member == null || !member.IsActive)
            {
                throw new DataNotFoundException("Member", memberId);
            }

            return member;
        }

        public Member GetMemberByEmail(string email)
        {
            Member? member = _memberRepository.GetMemberByEmail(email);

            if (member == null || !member.IsActive)
            {
                throw new DataNotFoundException("Member", "Email : " + email);
            }

            return member;
        }

        public Member GetMemberByPhoneNumber(string phoneNumber)
        {
            Member? member = _memberRepository.GetMemberByPhoneNumber(phoneNumber);

            if (member == null || !member.IsActive)
            {
                throw new DataNotFoundException("Member", "Phone Number : " + phoneNumber);
            }

            return member;
        }
    }
}