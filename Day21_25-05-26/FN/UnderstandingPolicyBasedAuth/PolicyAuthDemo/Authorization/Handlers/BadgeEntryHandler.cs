using Microsoft.AspNetCore.Authorization;
using PolicyAuthDemo.Authorization.Requirements;

namespace PolicyAuthDemo.Authorization.Handlers;

/// <summary>
/// Handler 1 of 2 for BuildingEntryRequirement.
/// 
/// Grants building access if user has a permanent "BadgeId" claim.
/// If this fails, TemporaryStickerHandler still gets a chance (OR logic).
/// </summary>
public class BadgeEntryHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    private readonly ILogger<BadgeEntryHandler> _logger;

    public BadgeEntryHandler(ILogger<BadgeEntryHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BuildingEntryRequirement requirement)
    {
        _logger.LogInformation("BadgeEntryHandler: Checking for permanent BadgeId claim");

        if (context.User.HasClaim(c => c.Type == "BadgeId"))
        {
            _logger.LogInformation("BadgeEntryHandler: Badge found ✅");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("BadgeEntryHandler: No badge — TemporaryStickerHandler will try next");
        }

        return Task.CompletedTask;
    }
}
