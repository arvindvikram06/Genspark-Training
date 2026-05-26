using Microsoft.AspNetCore.Authorization;

namespace PolicyAuthDemo.Authorization.Requirements;

/// <summary>
/// Requirement: User must be able to enter the building.
/// This requirement has NO data — it's a pure marker.
/// TWO handlers (BadgeEntryHandler + TemporaryStickerHandler) will
/// handle it, showing OR logic: if either handler succeeds, access is granted.
/// </summary>
public class BuildingEntryRequirement : IAuthorizationRequirement { }
