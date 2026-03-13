namespace joker_api.Models.DTOs;

public sealed class UserPreferencesDto
{
    public int Version { get; set; } = 1;
    public SidebarPreferencesDto Sidebar { get; set; } = new();
    public ChatWidgetPreferencesDto ChatWidget { get; set; } = new();
}

public sealed class SidebarPreferencesDto
{
    public bool Open { get; set; } = true;
    public string Variant { get; set; } = "inset";
    public List<string> PinnedItemIds { get; set; } = [];
}

public sealed class ChatWidgetPreferencesDto
{
    public string State { get; set; } = "open";
    public string Position { get; set; } = "bottom-right";
}