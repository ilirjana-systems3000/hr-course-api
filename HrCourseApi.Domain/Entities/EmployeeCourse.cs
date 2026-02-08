using HrCourseApi.Domain.Abstractions;

namespace HrCourseApi.Domain.Entities;

public sealed class EmployeeCourse : ITenantEntity
{
    public int EmployeeCourseId { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? Hours { get; set; }
    public decimal? Credits { get; set; }
    public decimal? DistrictCost { get; set; }
    public decimal? EmployeeCost { get; set; }

    public string? Grade { get; set; }
    public string? Major { get; set; }
    public string? Notes { get; set; }

    public int DistrictId { get; set; }
}
