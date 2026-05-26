using Microsoft.AspNetCore.Authorization;

namespace PolicyAuthDemo.Authorization.Requirements;

/// <summary>
/// Requirement: User must be at least X years old.
/// This is just a DATA class — it holds "what" is needed.
/// The actual logic lives in MinimumAgeHandler.
/// </summary>
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}
