using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.EmployeeCourses;

namespace HrCourseApi.Application.Abstractions.Services;

public interface IEmployeeCourseService
{
    Task<PagedResult<EmployeeCourseDto>> ListAsync(int? employeeId, int? courseId, int page, int pageSize, CancellationToken ct = default);
    Task<EmployeeCourseDto> AddAsync(UpsertEmployeeCourseRequest req, CancellationToken ct = default);
    Task<EmployeeCourseDto> EditAsync(int id, UpsertEmployeeCourseRequest req, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
