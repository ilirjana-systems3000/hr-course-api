using HrCourseApi.Application.Abstractions.Repositories;
using HrCourseApi.Application.Abstractions.Security;
using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Common.Errors;
using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.EmployeeCourses;
using HrCourseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Infrastructure.Services;

public sealed class EmployeeCourseService : IEmployeeCourseService
{
    private readonly IRepository<EmployeeCourse> _employeeCourses;
    private readonly IRepository<Employee> _employees;
    private readonly IRepository<Course> _courses;
    private readonly ITenantProvider _tenant;

    public EmployeeCourseService(
        IRepository<EmployeeCourse> employeeCourses,
        IRepository<Employee> employees,
        IRepository<Course> courses,
        ITenantProvider tenant)
    {
        _employeeCourses = employeeCourses;
        _employees = employees;
        _courses = courses;
        _tenant = tenant;
    }

    public async Task<PagedResult<EmployeeCourseDto>> ListAsync(int? employeeId, int? courseId, int page, int pageSize, CancellationToken ct = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 200);

        var q = _employeeCourses.Query().AsNoTracking();

        if (employeeId is not null)
            q = q.Where(x => x.EmployeeId == employeeId.Value);

        if (courseId is not null)
            q = q.Where(x => x.CourseId == courseId.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.EmployeeCourseId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EmployeeCourseDto(
                x.EmployeeCourseId, x.EmployeeId, x.CourseId, x.StartDate, x.EndDate,
                x.Hours, x.Credits, x.DistrictCost, x.EmployeeCost,
                x.Grade, x.Major, x.Notes
            ))
            .ToListAsync(ct);

        return new PagedResult<EmployeeCourseDto>(items, total, page, pageSize);
    }

    public async Task<EmployeeCourseDto> AddAsync(UpsertEmployeeCourseRequest req, CancellationToken ct = default)
    {
        await EnsureEmployeeExists(req.EmployeeId, ct);
        await EnsureCourseExists(req.CourseId, ct);

        var entity = Map(req, new EmployeeCourse());
        entity.DistrictId = _tenant.DistrictId;

        await _employeeCourses.AddAsync(entity, ct);
        await _employeeCourses.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task<EmployeeCourseDto> EditAsync(int id, UpsertEmployeeCourseRequest req, CancellationToken ct = default)
    {
        await EnsureEmployeeExists(req.EmployeeId, ct);
        await EnsureCourseExists(req.CourseId, ct);

        var entity = await _employeeCourses.GetByIdAsync(id, ct) ?? throw new DomainException("EmployeeCourse not found.");

        Map(req, entity);

        _employeeCourses.Update(entity);
        await _employeeCourses.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _employeeCourses.GetByIdAsync(id, ct) ?? throw new DomainException("EmployeeCourse not found.");
        _employeeCourses.Remove(entity);
        await _employeeCourses.SaveChangesAsync(ct);
    }

    private async Task EnsureEmployeeExists(int employeeId, CancellationToken ct)
    {
        var ok = await _employees.Query().AnyAsync(x => x.EmployeeId == employeeId, ct);
        if (!ok) throw new DomainException("Employee does not exist.");
    }

    private async Task EnsureCourseExists(int courseId, CancellationToken ct)
    {
        var ok = await _courses.Query().AnyAsync(x => x.CourseId == courseId, ct);
        if (!ok) throw new DomainException("Course does not exist.");
    }

    private static EmployeeCourse Map(UpsertEmployeeCourseRequest req, EmployeeCourse entity)
    {
        entity.EmployeeId = req.EmployeeId;
        entity.CourseId = req.CourseId;
        entity.StartDate = req.StartDate;
        entity.EndDate = req.EndDate;
        entity.Hours = req.Hours;
        entity.Credits = req.Credits;
        entity.DistrictCost = req.DistrictCost;
        entity.EmployeeCost = req.EmployeeCost;
        entity.Grade = req.Grade;
        entity.Major = req.Major;
        entity.Notes = req.Notes;
        return entity;
    }

    private static EmployeeCourseDto ToDto(EmployeeCourse x) => new(
        x.EmployeeCourseId, x.EmployeeId, x.CourseId, x.StartDate, x.EndDate,
        x.Hours, x.Credits, x.DistrictCost, x.EmployeeCost,
        x.Grade, x.Major, x.Notes
    );
}
