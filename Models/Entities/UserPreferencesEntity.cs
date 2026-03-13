namespace joker_api.Models.Entities;

public sealed class UserPreferencesEntity
{
    public string UserId { get; set; } = string.Empty;
    public string PreferencesJson { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAtUtc { get; set; }
}