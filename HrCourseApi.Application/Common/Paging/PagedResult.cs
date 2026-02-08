namespace HrCourseApi.Application.Common.Paging;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
