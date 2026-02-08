using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrCourseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HRUser")]
public sealed class CoursesController : ControllerBase
{
    private readonly ICourseService _svc;

    public CoursesController(ICourseService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDto>>> List(
        [FromQuery] int? courseTypeId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] bool? approved,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = new CourseFilter(courseTypeId, fromDate, toDate, approved);
        return Ok(await _svc.ListAsync(filter, page, pageSize, ct));
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Add([FromBody] UpsertCourseRequest req, CancellationToken ct)
        => Ok(await _svc.AddAsync(req, ct));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseDto>> Edit(int id, [FromBody] UpsertCourseRequest req, CancellationToken ct)
        => Ok(await _svc.EditAsync(id, req, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
