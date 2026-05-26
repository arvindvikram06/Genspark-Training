using Microsoft.AspNetCore.Authorization;
using PolicyAuthDemo.Authorization.Requirements;

namespace PolicyAuthDemo.Authorization.Handlers;

/// <summary>
/// Handler for DepartmentRequirement.
/// Checks if the user has a "Department" claim matching the required department.
/// </summary>
public class DepartmentHandler : AuthorizationHandler<DepartmentRequirement>
{
    private readonly ILogger<DepartmentHandler> _logger;

    public DepartmentHandler(ILogger<DepartmentHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DepartmentRequirement requirement)
    {
        _logger.LogInformation("DepartmentHandler: Checking department = {Dept}", requirement.RequiredDepartment);

        var departmentClaim = context.User.FindFirst("Department");

        if (departmentClaim is null)
        {
            _logger.LogWarning("DepartmentHandler: No Department claim found");
            return Task.CompletedTask;
        }

        if (string.Equals(departmentClaim.Value, requirement.RequiredDepartment, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("DepartmentHandler: Department matched ✅");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("DepartmentHandler: Department '{Actual}' does not match '{Required}'",
                departmentClaim.Value, requirement.RequiredDepartment);
        }

        return Task.CompletedTask;
    }
}
