using HrCourseApi.Domain.Abstractions;

namespace HrCourseApi.Domain.Entities;

public sealed class Course : ITenantEntity
{
    public int CourseId { get; set; }
    public string Description { get; set; } = string.Empty;

    public int CourseTypeId { get; set; }
    public CourseType? CourseType { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? Hours { get; set; }
    public decimal? Credits { get; set; }
    public decimal? DistrictCost { get; set; }
    public decimal? EmployeeCost { get; set; }
    public bool TuitionEligible { get; set; }
    public bool Approved { get; set; }
    public bool MaintenanceOfLicense { get; set; }

    public string? Provider { get; set; }
    public string? Presenter { get; set; }
    public string? Institution { get; set; }
    public string? Degree { get; set; }
    public string? CertNo { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }

    public int DistrictId { get; set; }

    public ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
}
