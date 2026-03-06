namespace joker_api.Models.Common;

public sealed class StonlyOptionsModel
{

    public const string SectionName = "StonlyOptions";
    public string BaseUrl { get; set; } = "https://public.stonly.com/api/v3/";
    public string AuthorizationHeader { get; set; } = Environment.GetEnvironmentVariable("STONLY_AUTHORIZATION_HEADER") ?? string.Empty;
}