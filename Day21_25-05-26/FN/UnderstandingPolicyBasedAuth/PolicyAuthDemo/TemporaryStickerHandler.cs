using Microsoft.AspNetCore.Authorization;
using PolicyAuthDemo.Authorization.Requirements;

namespace PolicyAuthDemo.Authorization.Handlers;

/// <summary>
/// Handler 2 of 2 for BuildingEntryRequirement.
/// 
/// Grants building access if user has a "TemporaryBadge" claim.
/// This is the fallback — even if BadgeEntryHandler failed,
/// this can still grant access (OR logic).
/// </summary>
public class TemporaryStickerHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    private readonly ILogger<TemporaryStickerHandler> _logger;

    public TemporaryStickerHandler(ILogger<TemporaryStickerHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BuildingEntryRequirement requirement)
    {
        _logger.LogInformation("TemporaryStickerHandler: Checking for TemporaryBadge claim");

        if (context.User.HasClaim(c => c.Type == "TemporaryBadge"))
        {
            _logger.LogInformation("TemporaryStickerHandler: Temporary badge found ✅");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("TemporaryStickerHandler: No temporary badge either — access denied");
        }

        return Task.CompletedTask;
    }
}
