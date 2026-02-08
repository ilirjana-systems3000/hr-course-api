using HrCourseApi.Application.Abstractions.Repositories;
using HrCourseApi.Application.Abstractions.Security;
using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Application.Common.Errors;
using HrCourseApi.Application.Features.CourseTypes;
using HrCourseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Infrastructure.Services;

public sealed class CourseTypeService : ICourseTypeService
{
    private readonly IRepository<CourseType> _courseTypes;
    private readonly IRepository<Course> _courses;
    private readonly ITenantProvider _tenant;

    public CourseTypeService(IRepository<CourseType> courseTypes, IRepository<Course> courses, ITenantProvider tenant)
    {
        _courseTypes = courseTypes;
        _courses = courses;
        _tenant = tenant;
    }

    public async Task<IReadOnlyList<CourseTypeDto>> ListAsync(CancellationToken ct = default)
    {
        var items = await _courseTypes.Query()
            .OrderBy(x => x.Description)
            .Select(x => new CourseTypeDto(x.CourseTypeId, x.Description))
            .ToListAsync(ct);

        return items;
    }

    public async Task<CourseTypeDto> AddAsync(UpsertCourseTypeRequest req, CancellationToken ct = default)
    {
        var desc = (req.Description ?? string.Empty).Trim();
        if (desc.Length == 0) throw new DomainException("Description is required.");

        var exists = await _courseTypes.AnyAsync(x => x.Description == desc, ct);
        if (exists) throw new DomainException("Duplicate CourseType description is not allowed.");

        var entity = new CourseType
        {
            Description = desc,
            DistrictId = _tenant.DistrictId
        };

        await _courseTypes.AddAsync(entity, ct);
        await _courseTypes.SaveChangesAsync(ct);

        return new CourseTypeDto(entity.CourseTypeId, entity.Description);
    }

    public async Task<CourseTypeDto> EditAsync(int id, UpsertCourseTypeRequest req, CancellationToken ct = default)
    {
        var entity = await _courseTypes.GetByIdAsync(id, ct) ?? throw new DomainException("CourseType not found.");

        var desc = (req.Description ?? string.Empty).Trim();
        if (desc.Length == 0) throw new DomainException("Description is required.");

        var dup = await _courseTypes.Query().AnyAsync(x => x.CourseTypeId != id && x.Description == desc, ct);
        if (dup) throw new DomainException("Duplicate CourseType description is not allowed.");

        entity.Description = desc;

        _courseTypes.Update(entity);
        await _courseTypes.SaveChangesAsync(ct);

        return new CourseTypeDto(entity.CourseTypeId, entity.Description);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _courseTypes.GetByIdAsync(id, ct) ?? throw new DomainException("CourseType not found.");

        var used = await _courses.Query().AnyAsync(c => c.CourseTypeId == id, ct);
        if (used) throw new DomainException("Cannot delete CourseType because it is used by a Course.");

        _courseTypes.Remove(entity);
        await _courseTypes.SaveChangesAsync(ct);
    }
}
