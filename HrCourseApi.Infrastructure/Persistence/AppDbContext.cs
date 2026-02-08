using HrCourseApi.Application.Abstractions.Security;
using HrCourseApi.Domain.Abstractions;
using HrCourseApi.Domain.Entities;
using HrCourseApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantProvider _tenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenant) : base(options)
    {
        _tenant = tenant;
    }

    public DbSet<CourseType> CourseTypes => Set<CourseType>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeCourse> EmployeeCourses => Set<EmployeeCourse>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Tenant query filters
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext).GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { builder, this });
            }
        }

        builder.Entity<CourseType>(b =>
        {
            b.HasKey(x => x.CourseTypeId);
            b.Property(x => x.Description).HasMaxLength(200).IsRequired();

            // Unique per district (prevents duplicates)
            b.HasIndex(x => new { x.DistrictId, x.Description }).IsUnique();
        });

        builder.Entity<Course>(b =>
        {
            b.HasKey(x => x.CourseId);
            b.Property(x => x.Description).HasMaxLength(200).IsRequired();

            b.HasOne(x => x.CourseType)
             .WithMany(x => x.Courses)
             .HasForeignKey(x => x.CourseTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            // Basic duplicate prevention per district; adjust if you choose a different uniqueness definition.
            b.HasIndex(x => new { x.DistrictId, x.CourseTypeId, x.Description }).IsUnique();
        });

        builder.Entity<Employee>(b =>
        {
            b.HasKey(x => x.EmployeeId);
            b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();

            b.HasIndex(x => new { x.DistrictId, x.Email }).IsUnique();
        });

        builder.Entity<EmployeeCourse>(b =>
        {
            b.HasKey(x => x.EmployeeCourseId);

            b.HasOne(x => x.Employee)
             .WithMany(x => x.EmployeeCourses)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Course)
             .WithMany(x => x.EmployeeCourses)
             .HasForeignKey(x => x.CourseId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void SetTenantFilter<TEntity>(ModelBuilder builder, AppDbContext ctx)
        where TEntity : class, ITenantEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => e.DistrictId == ctx._tenant.DistrictId);
    }
}
