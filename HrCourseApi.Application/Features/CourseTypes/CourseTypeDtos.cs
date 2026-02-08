namespace HrCourseApi.Application.Features.CourseTypes;

public sealed record CourseTypeDto(int CourseTypeId, string Description);

public sealed record UpsertCourseTypeRequest(string Description);
