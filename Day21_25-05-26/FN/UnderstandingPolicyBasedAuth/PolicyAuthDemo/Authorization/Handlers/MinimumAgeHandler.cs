using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PolicyAuthDemo.Authorization.Requirements;

namespace PolicyAuthDemo.Authorization.Handlers;

/// <summary>
/// Handler for MinimumAgeRequirement.
/// Reads the "DateOfBirth" claim and checks if user meets the minimum age.
/// 
/// Rules:
///   - If the claim is missing → do nothing (don't call Succeed or Fail)
///   - If age is sufficient    → context.Succeed(requirement)
///   - If age is insufficient  → do nothing (other handlers could still succeed,
///                               but there are none here, so it will implicitly fail)
/// </summary>
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    private readonly ILogger<MinimumAgeHandler> _logger;

    public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        _logger.LogInformation("MinimumAgeHandler: Checking age requirement of {Age}", requirement.MinimumAge);

        // Look for a DateOfBirth claim
        var dobClaim = context.User.FindFirst(ClaimTypes.DateOfBirth);

        if (dobClaim is null)
        {
            _logger.LogWarning("MinimumAgeHandler: No DateOfBirth claim found — failing silently");
            return Task.CompletedTask; // No claim → requirement not met
        }

        var dateOfBirth = DateTime.Parse(dobClaim.Value);
        int age = DateTime.Today.Year - dateOfBirth.Year;

        // Adjust for birthday not yet occurred this year
        if (dateOfBirth > DateTime.Today.AddYears(-age))
            age--;

        _logger.LogInformation("MinimumAgeHandler: Calculated age = {Age}, required = {Required}", age, requirement.MinimumAge);

        if (age >= requirement.MinimumAge)
        {
            context.Succeed(requirement); // ✅ Pass!
        }

        return Task.CompletedTask;
    }
}
