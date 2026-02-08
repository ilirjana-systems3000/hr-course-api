namespace HrCourseApi.Api;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "HrCourseApi";
    public string Audience { get; set; } = "HrCourseApi";
    public string Key { get; set; } = "DEV_ONLY_CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_CHANGE_ME_1234567890";
    public int AccessTokenMinutes { get; set; } = 60;
}
