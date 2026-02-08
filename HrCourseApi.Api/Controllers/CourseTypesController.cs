using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Features.CourseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrCourseApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HRUser")]
public sealed class CourseTypesController : ControllerBase
{
    private readonly ICourseTypeService _svc;

    public CourseTypesController(ICourseTypeService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseTypeDto>>> List(CancellationToken ct)
        => Ok(await _svc.ListAsync(ct));

    [HttpPost]
    public async Task<ActionResult<CourseTypeDto>> Add([FromBody] UpsertCourseTypeRequest req, CancellationToken ct)
        => Ok(await _svc.AddAsync(req, ct));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseTypeDto>> Edit(int id, [FromBody] UpsertCourseTypeRequest req, CancellationToken ct)
        => Ok(await _svc.EditAsync(id, req, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
