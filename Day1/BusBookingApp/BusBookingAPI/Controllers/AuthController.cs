using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            if (result.Message == "Email already exists")
                return Conflict(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }
        return Ok(result.Data);
    }

    [HttpPost("operator/register")]
    public async Task<IActionResult> RegisterOperator(OperatorRegisterRequest request)
    {
        var result = await _authService.RegisterOperatorAsync(request);
        if (!result.Success)
        {
            if (result.Message == "Email already exists")
                return Conflict(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }
        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }
        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}
