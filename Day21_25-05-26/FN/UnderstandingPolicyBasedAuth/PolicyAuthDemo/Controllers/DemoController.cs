using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolicyAuthDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    // ─────────────────────────────────────────────────────────────────
    // PUBLIC — no auth required
    // ─────────────────────────────────────────────────────────────────

    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(new
        {
            message = "Anyone can access this endpoint.",
            yourClaims = claims
        });
    }

    // ─────────────────────────────────────────────────────────────────
    // POLICY 1: AtLeast18
    // Single requirement: MinimumAgeRequirement(18)
    // Handler: MinimumAgeHandler checks DateOfBirth claim
    // ─────────────────────────────────────────────────────────────────

    [HttpGet("adults-only")]
    [Authorize(Policy = "AtLeast18")]
    public IActionResult AdultsOnly()
    {
        return Ok(new
        {
            message = "✅ You are 18+. Welcome to the adults section.",
            user = User.Identity?.Name
        });
    }

    // ─────────────────────────────────────────────────────────────────
    // POLICY 2: SeniorEngineer
    // TWO requirements: MinimumAgeRequirement(30) AND DepartmentRequirement("Engineering")
    // Both must pass (AND logic)
    // ─────────────────────────────────────────────────────────────────

    [HttpGet("senior-engineer")]
    [Authorize(Policy = "SeniorEngineer")]
    public IActionResult SeniorEngineer()
    {
        return Ok(new
        {
            message = "✅ You are 30+ AND in Engineering. Senior engineer area.",
            user = User.Identity?.Name
        });
    }

    // ─────────────────────────────────────────────────────────────────
    // POLICY 3: CanEnterBuilding
    // ONE requirement: BuildingEntryRequirement
    // TWO handlers: BadgeEntryHandler OR TemporaryStickerHandler
    // Only ONE handler needs to succeed (OR logic)
    // ─────────────────────────────────────────────────────────────────

    [HttpGet("building-entry")]
    [Authorize(Policy = "CanEnterBuilding")]
    public IActionResult BuildingEntry()
    {
        return Ok(new
        {
            message = "✅ Building access granted (either badge or temporary sticker worked).",
            user = User.Identity?.Name
        });
    }

    // ─────────────────────────────────────────────────────────────────
    // POLICY 4: Simple inline assertion (no separate handler class needed)
    // ─────────────────────────────────────────────────────────────────

    [HttpGet("hr-only")]
    [Authorize(Policy = "HRDepartment")]
    public IActionResult HrOnly()
    {
        return Ok(new
        {
            message = "✅ You are in HR. Sensitive data unlocked.",
            user = User.Identity?.Name
        });
    }
}
