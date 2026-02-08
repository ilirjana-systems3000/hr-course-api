using HrCourseApi.Domain.Abstractions;

namespace HrCourseApi.Domain.Entities;

public sealed class Employee : ITenantEntity
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int DistrictId { get; set; }

    public ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
}
