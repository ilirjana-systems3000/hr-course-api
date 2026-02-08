namespace HrCourseApi.Application.Features.EmployeeCourses;

public sealed record EmployeeDto(int EmployeeId, string FirstName, string LastName, string Email);

public sealed record EmployeeCourseDto(
    int EmployeeCourseId,
    int EmployeeId,
    int CourseId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? Hours,
    decimal? Credits,
    decimal? DistrictCost,
    decimal? EmployeeCost,
    string? Grade,
    string? Major,
    string? Notes
);

public sealed record UpsertEmployeeCourseRequest(
    int EmployeeId,
    int CourseId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? Hours,
    decimal? Credits,
    decimal? DistrictCost,
    decimal? EmployeeCost,
    string? Grade,
    string? Major,
    string? Notes
);
