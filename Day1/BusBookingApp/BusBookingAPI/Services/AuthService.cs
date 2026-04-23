using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace BusBookingAPI.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, AuthResponse? Data)> RegisterAsync(RegisterRequest request);
    Task<(bool Success, string Message, AuthResponse? Data)> RegisterOperatorAsync(OperatorRegisterRequest request);
    Task<(bool Success, string Message, AuthResponse? Data)> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly BusBookingDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(BusBookingDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message, AuthResponse? Data)> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return (false, "Email already exists", null);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BC.HashPassword(request.Password),
            Phone = request.Phone,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return (true, "Registration successful", new AuthResponse(token, user.Id, user.Email, user.Role.ToString()));
    }

    public async Task<(bool Success, string Message, AuthResponse? Data)> RegisterOperatorAsync(OperatorRegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return (false, "Email already exists", null);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password),
                Phone = request.Phone,
                Role = UserRole.Operator,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var op = new Operator
            {
                UserId = user.Id,
                Status = OperatorStatus.Pending,
                HeadOfficeDistrict = request.HeadOfficeDistrict
            };

            _context.Operators.Add(op);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var token = GenerateJwtToken(user);
            return (true, "Operator registration successful", new AuthResponse(token, user.Id, user.Email, user.Role.ToString()));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error occurred: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, AuthResponse? Data)> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            return (false, "Invalid credentials", null);

        if (!user.IsActive)
            return (false, "Account is disabled", null);

        var token = GenerateJwtToken(user);
        return (true, "Login successful", new AuthResponse(token, user.Id, user.Email, user.Role.ToString()));
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTAuthentication123456"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "BusBookingAPI",
            audience: _configuration["Jwt:Audience"] ?? "BusBookingUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
