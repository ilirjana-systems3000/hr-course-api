namespace HrCourseApi.Application.Features.Courses;

public sealed record CourseDto(
    int CourseId,
    string Description,
    int CourseTypeId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? Hours,
    decimal? Credits,
    decimal? DistrictCost,
    decimal? EmployeeCost,
    bool TuitionEligible,
    bool Approved,
    bool MaintenanceOfLicense,
    string? Provider,
    string? Presenter,
    string? Institution,
    string? Degree,
    string? CertNo,
    string? Location,
    string? Notes
);

public sealed record UpsertCourseRequest(
    string Description,
    int CourseTypeId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? Hours,
    decimal? Credits,
    decimal? DistrictCost,
    decimal? EmployeeCost,
    bool TuitionEligible,
    bool Approved,
    bool MaintenanceOfLicense,
    string? Provider,
    string? Presenter,
    string? Institution,
    string? Degree,
    string? CertNo,
    string? Location,
    string? Notes
);

public sealed record CourseFilter(
    int? CourseTypeId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    bool? Approved
);
