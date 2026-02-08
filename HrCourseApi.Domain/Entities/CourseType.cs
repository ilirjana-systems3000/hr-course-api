using HrCourseApi.Domain.Abstractions;

namespace HrCourseApi.Domain.Entities;

public sealed class CourseType : ITenantEntity
{
    public int CourseTypeId { get; set; }
    public string Description { get; set; } = string.Empty;

    public int DistrictId { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
