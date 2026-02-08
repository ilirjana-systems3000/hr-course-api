namespace HrCourseApi.Application.Abstractions.Security;

public interface ITenantProvider
{
    /// <summary>District/Tenant id (LeapProfileId).</summary>
    int DistrictId { get; }
}
