using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.EmployeeCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrCourseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HRUser")]
public sealed class EmployeeCoursesController : ControllerBase
{
    private readonly IEmployeeCourseService _svc;

    public EmployeeCoursesController(IEmployeeCourseService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeCourseDto>>> List(
        [FromQuery] int? employeeId,
        [FromQuery] int? courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _svc.ListAsync(employeeId, courseId, page, pageSize, ct));

    [HttpPost]
    public async Task<ActionResult<EmployeeCourseDto>> Add([FromBody] UpsertEmployeeCourseRequest req, CancellationToken ct)
        => Ok(await _svc.AddAsync(req, ct));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeCourseDto>> Edit(int id, [FromBody] UpsertEmployeeCourseRequest req, CancellationToken ct)
        => Ok(await _svc.EditAsync(id, req, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
