# HrCourseApi

A Clean Architecture ASP.NET Core Web API for managing HR Courses with:

- **Clean Architecture**: Domain / Application / Infrastructure / API projects
- **SQL Server + EF Core**
- **ASP.NET Identity**
- **JWT Authentication**
- **Role-based Authorization**: `Admin`, `HRUser`
- **Multi-tenant isolation by District** using `LeapProfileId` (stored on the user and included as a JWT claim)
- Business rules:
  - No duplicate `CourseType.Description` within the same district
  - Cannot delete `CourseType` if used by `Course`
  - No duplicate `Course` within the same district
  - Cannot delete `Course` if used by `EmployeeCourse`
  - Filtering courses by type/date/approved

---

## Project Structure


```text
src/
  HrCourseApi.Api/
    HrCourseApi.Api.csproj
    Program.cs
    appsettings.json
  HrCourseApi.Application/
    HrCourseApi.Application.csproj
  HrCourseApi.Domain/
    HrCourseApi.Domain.csproj
  HrCourseApi.Infrastructure/
    HrCourseApi.Infrastructure.csproj
