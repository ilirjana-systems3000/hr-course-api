using HrCourseApi.Domain.Entities;
using HrCourseApi.Infrastructure.Identity;
using HrCourseApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Api.Seed;

public sealed class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly UserManager<ApplicationUser> _users;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext db, RoleManager<IdentityRole> roles, UserManager<ApplicationUser> users, ILogger<DbSeeder> logger)
    {
        _db = db;
        _roles = roles;
        _users = users;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _db.Database.MigrateAsync(ct);

        await EnsureRoleAsync("Admin");
        await EnsureRoleAsync("HRUser");

        await EnsureUserAsync("admin1@demo.com", "Passw0rd!", leapProfileId: 1, role: "Admin");
        await EnsureUserAsync("hr1@demo.com", "Passw0rd!", leapProfileId: 1, role: "HRUser");
        await EnsureUserAsync("admin2@demo.com", "Passw0rd!", leapProfileId: 2, role: "Admin");
        await EnsureUserAsync("hr2@demo.com", "Passw0rd!", leapProfileId: 2, role: "HRUser");

        // Seed domain data for both districts (bypass tenant filter by using IgnoreQueryFilters where needed)
        await SeedDistrictAsync(1, ct);
        await SeedDistrictAsync(2, ct);
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (!await _roles.RoleExistsAsync(roleName))
        {
            var res = await _roles.CreateAsync(new IdentityRole(roleName));
            if (!res.Succeeded)
                throw new InvalidOperationException("Failed to create role: " + roleName);
        }
    }

    private async Task EnsureUserAsync(string email, string password, int leapProfileId, string role)
    {
        var user = await _users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LeapProfileId = leapProfileId
            };

            var res = await _users.CreateAsync(user, password);
            if (!res.Succeeded)
                throw new InvalidOperationException("Failed to create user: " + email + " :: " + string.Join(", ", res.Errors.Select(e => e.Description)));
        }

        if (!await _users.IsInRoleAsync(user, role))
        {
            var res = await _users.AddToRoleAsync(user, role);
            if (!res.Succeeded)
                throw new InvalidOperationException("Failed to add user to role: " + email + " -> " + role);
        }
    }

    private async Task SeedDistrictAsync(int districtId, CancellationToken ct)
    {
        // use IgnoreQueryFilters to check across tenants during seeding
        var hasAny = await _db.CourseTypes.IgnoreQueryFilters().AnyAsync(x => x.DistrictId == districtId, ct);
        if (hasAny) return;

        var ct1 = new CourseType { Description = "Safety Training", DistrictId = districtId };
        var ct2 = new CourseType { Description = "Professional Development", DistrictId = districtId };

        _db.CourseTypes.AddRange(ct1, ct2);
        await _db.SaveChangesAsync(ct);

        var course1 = new Course
        {
            Description = "Workplace Safety 101",
            CourseTypeId = ct1.CourseTypeId,
            StartDate = new DateOnly(2026, 2, 1),
            EndDate = new DateOnly(2026, 2, 2),
            Hours = 4,
            Credits = 0,
            DistrictCost = 0,
            EmployeeCost = 0,
            TuitionEligible = false,
            Approved = true,
            MaintenanceOfLicense = false,
            Provider = "Internal",
            Location = "Online",
            Notes = "Seeded data",
            DistrictId = districtId
        };

        _db.Courses.Add(course1);

        var emp = new Employee
        {
            FirstName = "Demo",
            LastName = $"Employee{districtId}",
            Email = $"employee{districtId}@demo.com",
            DistrictId = districtId
        };

        _db.Employees.Add(emp);
        await _db.SaveChangesAsync(ct);

        _db.EmployeeCourses.Add(new EmployeeCourse
        {
            EmployeeId = emp.EmployeeId,
            CourseId = course1.CourseId,
            StartDate = course1.StartDate,
            EndDate = course1.EndDate,
            Hours = course1.Hours,
            Credits = course1.Credits,
            DistrictCost = course1.DistrictCost,
            EmployeeCost = course1.EmployeeCost,
            Grade = "Pass",
            Notes = "Seeded relation",
            DistrictId = districtId
        });

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Seeded data for district {DistrictId}", districtId);
    }
}
