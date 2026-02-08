using System.Security.Claims;
using HrCourseApi.Application.Abstractions.Security;

namespace HrCourseApi.Api.Security;

public sealed class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _http;

    public HttpTenantProvider(IHttpContextAccessor http) => _http = http;

    public int DistrictId
    {
        get
        {
            // In this project, DistrictId == user's LeapProfileId (per requirements).
            var user = _http.HttpContext?.User;
            if (user is null || !user.Identity?.IsAuthenticated == true)
                return 0;

            var v = user.FindFirstValue("LeapProfileId");
            return int.TryParse(v, out var id) ? id : 0;
        }
    }
}
