namespace HrCourseApi.Application.Common.Errors;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
