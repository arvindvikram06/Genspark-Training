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

    [HttpPost("register/user")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }
        return Ok(new ApiResponse<AuthResponse>(true, result.Data, result.Message));
    }

    [HttpPost("register/operator")]
    public async Task<IActionResult> RegisterOperator(OperatorRegisterRequest request)
    {
        var result = await _authService.RegisterOperatorAsync(request);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }
        return Ok(new ApiResponse<AuthResponse>(true, result.Data, result.Message));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return Unauthorized(new ApiResponse<object>(false, null, result.Message));
        }
        return Ok(new ApiResponse<AuthResponse>(true, result.Data, result.Message));
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}
