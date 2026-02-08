using HrCourseApi.Application.Abstractions.Repositories;
using HrCourseApi.Application.Abstractions.Services;
using HrCourseApi.Infrastructure.Persistence;
using HrCourseApi.Infrastructure.Repositories;
using HrCourseApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrCourseApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ICourseTypeService, CourseTypeService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IEmployeeCourseService, EmployeeCourseService>();

        return services;
    }
}
