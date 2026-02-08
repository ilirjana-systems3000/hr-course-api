using HrCourseApi.Application.Abstractions.Repositories;
using HrCourseApi.Application.Abstractions.Security;
using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Common.Errors;
using HrCourseApi.Application.Common.Paging;
using HrCourseApi.Application.Features.Courses;
using HrCourseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Infrastructure.Services;

public sealed class CourseService : ICourseService
{
    private readonly IRepository<Course> _courses;
    private readonly IRepository<CourseType> _courseTypes;
    private readonly IRepository<EmployeeCourse> _employeeCourses;
    private readonly ITenantProvider _tenant;

    public CourseService(
        IRepository<Course> courses,
        IRepository<CourseType> courseTypes,
        IRepository<EmployeeCourse> employeeCourses,
        ITenantProvider tenant)
    {
        _courses = courses;
        _courseTypes = courseTypes;
        _employeeCourses = employeeCourses;
        _tenant = tenant;
    }

    public async Task<PagedResult<CourseDto>> ListAsync(CourseFilter filter, int page, int pageSize, CancellationToken ct = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 200);

        var q = _courses.Query().AsNoTracking();

        if (filter.CourseTypeId is not null)
            q = q.Where(x => x.CourseTypeId == filter.CourseTypeId.Value);

        if (filter.Approved is not null)
            q = q.Where(x => x.Approved == filter.Approved.Value);

        if (filter.FromDate is not null)
            q = q.Where(x => x.StartDate == null || x.StartDate.Value >= filter.FromDate.Value);

        if (filter.ToDate is not null)
            q = q.Where(x => x.EndDate == null || x.EndDate.Value <= filter.ToDate.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.CourseId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CourseDto(
                x.CourseId, x.Description, x.CourseTypeId, x.StartDate, x.EndDate,
                x.Hours, x.Credits, x.DistrictCost, x.EmployeeCost,
                x.TuitionEligible, x.Approved, x.MaintenanceOfLicense,
                x.Provider, x.Presenter, x.Institution, x.Degree, x.CertNo, x.Location, x.Notes
            ))
            .ToListAsync(ct);

        return new PagedResult<CourseDto>(items, total, page, pageSize);
    }

    public async Task<CourseDto> AddAsync(UpsertCourseRequest req, CancellationToken ct = default)
    {
        await EnsureCourseTypeExists(req.CourseTypeId, ct);

        var desc = (req.Description ?? string.Empty).Trim();
        if (desc.Length == 0) throw new DomainException("Description is required.");

        // Duplicate rule (by district + type + description, matches DB unique index)
        var exists = await _courses.AnyAsync(x => x.CourseTypeId == req.CourseTypeId && x.Description == desc, ct);
        if (exists) throw new DomainException("Duplicate Course is not allowed.");

        var entity = Map(req, new Course());
        entity.Description = desc;
        entity.DistrictId = _tenant.DistrictId;

        await _courses.AddAsync(entity, ct);
        await _courses.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task<CourseDto> EditAsync(int id, UpsertCourseRequest req, CancellationToken ct = default)
    {
        await EnsureCourseTypeExists(req.CourseTypeId, ct);

        var entity = await _courses.GetByIdAsync(id, ct) ?? throw new DomainException("Course not found.");

        var desc = (req.Description ?? string.Empty).Trim();
        if (desc.Length == 0) throw new DomainException("Description is required.");

        var dup = await _courses.Query().AnyAsync(x =>
            x.CourseId != id &&
            x.CourseTypeId == req.CourseTypeId &&
            x.Description == desc, ct);

        if (dup) throw new DomainException("Duplicate Course is not allowed.");

        Map(req, entity);
        entity.Description = desc;

        _courses.Update(entity);
        await _courses.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _courses.GetByIdAsync(id, ct) ?? throw new DomainException("Course not found.");

        var used = await _employeeCourses.Query().AnyAsync(ec => ec.CourseId == id, ct);
        if (used) throw new DomainException("Cannot delete Course because it is used by EmployeeCourse.");

        _courses.Remove(entity);
        await _courses.SaveChangesAsync(ct);
    }

    private async Task EnsureCourseTypeExists(int courseTypeId, CancellationToken ct)
    {
        var ok = await _courseTypes.Query().AnyAsync(x => x.CourseTypeId == courseTypeId, ct);
        if (!ok) throw new DomainException("CourseType does not exist.");
    }

    private static Course Map(UpsertCourseRequest req, Course entity)
    {
        entity.CourseTypeId = req.CourseTypeId;
        entity.StartDate = req.StartDate;
        entity.EndDate = req.EndDate;
        entity.Hours = req.Hours;
        entity.Credits = req.Credits;
        entity.DistrictCost = req.DistrictCost;
        entity.EmployeeCost = req.EmployeeCost;
        entity.TuitionEligible = req.TuitionEligible;
        entity.Approved = req.Approved;
        entity.MaintenanceOfLicense = req.MaintenanceOfLicense;
        entity.Provider = req.Provider;
        entity.Presenter = req.Presenter;
        entity.Institution = req.Institution;
        entity.Degree = req.Degree;
        entity.CertNo = req.CertNo;
        entity.Location = req.Location;
        entity.Notes = req.Notes;
        return entity;
    }

    private static CourseDto ToDto(Course x) => new(
        x.CourseId, x.Description, x.CourseTypeId, x.StartDate, x.EndDate,
        x.Hours, x.Credits, x.DistrictCost, x.EmployeeCost,
        x.TuitionEligible, x.Approved, x.MaintenanceOfLicense,
        x.Provider, x.Presenter, x.Institution, x.Degree, x.CertNo, x.Location, x.Notes
    );
}
