namespace HrCourseApi.Application.Abstractions.Security;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
}
