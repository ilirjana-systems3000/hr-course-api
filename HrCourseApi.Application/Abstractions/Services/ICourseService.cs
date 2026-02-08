using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.Courses;

namespace HrCourseApi.Application.Abstractions.Services;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> ListAsync(CourseFilter filter, int page, int pageSize, CancellationToken ct = default);
    Task<CourseDto> AddAsync(UpsertCourseRequest req, CancellationToken ct = default);
    Task<CourseDto> EditAsync(int id, UpsertCourseRequest req, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
