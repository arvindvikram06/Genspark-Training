using Microsoft.AspNetCore.Authorization;

namespace PolicyAuthDemo.Authorization.Requirements;

/// <summary>
/// Requirement: User must belong to a specific department.
/// Used in the "SeniorEmployee" policy alongside MinimumAgeRequirement
/// to show AND logic (both requirements must pass).
/// </summary>
public class DepartmentRequirement : IAuthorizationRequirement
{
    public string RequiredDepartment { get; }

    public DepartmentRequirement(string department)
    {
        RequiredDepartment = department;
    }
}
