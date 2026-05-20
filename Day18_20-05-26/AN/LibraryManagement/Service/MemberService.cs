using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Repository;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public Member AddMember(Member member)
        {
            member.MemberId = 0;
            if (string.IsNullOrWhiteSpace(member.FullName))
            {
                throw new InvalidInputException("Member full name should not be empty.");
            }

            if (string.IsNullOrWhiteSpace(member.Email))
            {
                throw new InvalidInputException("Email should not be empty.");
            }

            if (string.IsNullOrWhiteSpace(member.PhoneNumber))
            {
                throw new InvalidInputException("Phone number should not be empty.");
            }

            if (member.MembershipDate == default)
            {
                member.MembershipDate = DateTime.UtcNow;
            }
            else
            {
                member.MembershipDate = DateTime.SpecifyKind(member.MembershipDate, DateTimeKind.Utc);
            }

            return _memberRepository.Add(member);
        }

        public IEnumerable<Member> GetAllMembers()
        {
            return _memberRepository.GetAll();
        }

        public Member GetMemberById(int id)
        {
            var member = _memberRepository.GetById(id);
            if (member == null)
            {
                throw new EntityNotFoundException("Member", id);
            }
            return member;
        }
    }
}
