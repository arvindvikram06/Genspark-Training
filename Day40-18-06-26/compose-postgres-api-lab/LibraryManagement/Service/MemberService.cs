using BankingAPI.Interfaces;
using BankingAPI.Models.DTOs;
using LibraryManagement.DTOs;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Repository;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        private readonly ITokenService _tokenService;

        public MemberService(IMemberRepository memberRepository, ITokenService tokenService)
        {
            _memberRepository = memberRepository;
             _tokenService = tokenService;
        }

        public Member AddMember(Member member, string password)
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

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidInputException("Password should not be empty.");
            }

            if (string.IsNullOrWhiteSpace(member.PhoneNumber))
            {
                throw new InvalidInputException("Phone number should not be empty.");
            }

            if (_memberRepository.GetByEmail(member.Email) != null)
            {
                throw new InvalidInputException("A member with this email already exists.");
            }

            if (member.MembershipDate == default)
            {
                member.MembershipDate = DateTime.UtcNow;
            }
            else
            {
                member.MembershipDate = DateTime.SpecifyKind(member.MembershipDate, DateTimeKind.Utc);
            }

            PasswordHasher.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            member.PasswordHash = passwordHash;
            member.PasswordSalt = passwordSalt;

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

        public LoginResponse Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidInputException("Email should not be empty.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidInputException("Password should not be empty.");
            }

            var member = _memberRepository.GetByEmail(email);
            if (member == null)
            {
                throw new InvalidInputException("Invalid email or password.");
            }

            if (!PasswordHasher.VerifyPasswordHash(password, member.PasswordHash, member.PasswordSalt))
            {
                throw new InvalidInputException("Invalid email or password.");
            }

            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Name = member.Email;

            loginResponse.Token = _tokenService.CreateNewToken(new TokenRequest
            {
                Email = member.Email,
                Name = member.FullName
            });


            return loginResponse;
        }
    }
}
