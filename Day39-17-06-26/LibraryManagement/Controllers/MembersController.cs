using LibraryManagement.DTOs;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryManagement.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetAllMembers()
        {
            try
            {
                var members = _memberService.GetAllMembers();
                return Ok(members);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Member> GetMemberById(int id)
        {
            try
            {
                var member = _memberService.GetMemberById(id);
                return Ok(member);
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
