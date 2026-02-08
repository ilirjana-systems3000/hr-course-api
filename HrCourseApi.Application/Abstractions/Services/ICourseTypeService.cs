using HrCourseApi.Application.Features.CourseTypes;

namespace HrCourseApi.Application.Abstractions.Services;

public interface ICourseTypeService
{
    Task<IReadOnlyList<CourseTypeDto>> ListAsync(CancellationToken ct = default);
    Task<CourseTypeDto> AddAsync(UpsertCourseTypeRequest req, CancellationToken ct = default);
    Task<CourseTypeDto> EditAsync(int id, UpsertCourseTypeRequest req, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
