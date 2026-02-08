using Microsoft.AspNetCore.Identity;

namespace HrCourseApi.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    /// <summary>Represents the district/tenant (LeapProfileId requirement).</summary>
    public int LeapProfileId { get; set; }
}
