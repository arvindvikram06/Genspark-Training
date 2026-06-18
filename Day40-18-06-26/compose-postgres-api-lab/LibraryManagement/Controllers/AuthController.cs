using LibraryManagement.DTOs;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public AuthController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateMemberDto dto)
        {
            try
            {
                var member = new Member
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    MembershipDate = dto.MembershipDate
                };
                _memberService.AddMember(member, dto.Password);
                return Ok(new { message = "Member added successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = _memberService.Login(dto.Email, dto.Password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

          private ActionResult HandleException(Exception ex)
        {
            if (ex is EntityNotFoundException)
            {
                return NotFound(new { message = ex.Message });
            }
            if (ex is InvalidInputException)
            {
                return BadRequest(new { message = ex.Message });
            }
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
