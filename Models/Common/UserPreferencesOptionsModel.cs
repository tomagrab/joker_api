namespace joker_api.Models.Common;

public sealed class UserPreferencesOptionsModel
{
    public const string SectionName = "UserPreferences";

    public bool AllowAnonymousDevelopmentUser { get; set; }
    public string DevelopmentUserId { get; set; } = "local-development-user";
}