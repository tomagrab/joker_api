using System.Text.Json;
using joker_api.Data.Context;
using joker_api.Models.DTOs;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace joker_api.Services.Services;

public sealed class UserPreferencesService(ILogger<UserPreferencesService> logger, AppDbContext context) : IUserPreferencesService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly HashSet<string> ValidSidebarVariants = ["sidebar", "floating", "inset"];
    private static readonly HashSet<string> ValidSidebarItemIds = ["home", "tools", "settings.app", "settings.user"];
    private static readonly HashSet<string> ValidChatWidgetStates = ["closed", "open", "fullscreen"];
    private static readonly HashSet<string> ValidChatWidgetPositions = ["top-left", "top-right", "bottom-left", "bottom-right"];

    private readonly ILogger<UserPreferencesService> _logger = logger;
    private readonly AppDbContext _context = context;

    public async Task<UserPreferencesDto> GetCurrentUserPreferencesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.UserPreferences
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (entity is null)
        {
            return CreateDefaultPreferences();
        }

        return DeserializeAndNormalize(entity.PreferencesJson, userId);
    }

    public async Task<UserPreferencesDto> UpdateCurrentUserPreferencesAsync(string userId, UserPreferencesDto preferences, CancellationToken cancellationToken = default)
    {
        var normalizedPreferences = NormalizePreferences(preferences);
        var serializedPreferences = JsonSerializer.Serialize(normalizedPreferences, SerializerOptions);

        var entity = await _context.UserPreferences
            .SingleOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (entity is null)
        {
            entity = new UserPreferencesEntity
            {
                UserId = userId
            };

            _context.UserPreferences.Add(entity);
        }

        entity.PreferencesJson = serializedPreferences;
        entity.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return normalizedPreferences;
    }

    private UserPreferencesDto DeserializeAndNormalize(string preferencesJson, string userId)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<UserPreferencesDto>(preferencesJson, SerializerOptions);
            return NormalizePreferences(deserialized);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Stored user preferences JSON for user {UserId} could not be parsed. Falling back to defaults.", userId);
            return CreateDefaultPreferences();
        }
    }

    private static UserPreferencesDto NormalizePreferences(UserPreferencesDto? preferences)
    {
        var defaults = CreateDefaultPreferences();
        var sidebar = preferences?.Sidebar;
        var chatWidget = preferences?.ChatWidget;

        return new UserPreferencesDto
        {
            Version = 1,
            Sidebar = new SidebarPreferencesDto
            {
                Open = sidebar?.Open ?? defaults.Sidebar.Open,
                Variant = NormalizeEnumValue(sidebar?.Variant, ValidSidebarVariants, defaults.Sidebar.Variant),
                PinnedItemIds = NormalizePinnedItemIds(sidebar?.PinnedItemIds)
            },
            ChatWidget = new ChatWidgetPreferencesDto
            {
                State = NormalizeEnumValue(chatWidget?.State, ValidChatWidgetStates, defaults.ChatWidget.State),
                Position = NormalizeEnumValue(chatWidget?.Position, ValidChatWidgetPositions, defaults.ChatWidget.Position)
            }
        };
    }

    private static List<string> NormalizePinnedItemIds(IEnumerable<string>? pinnedItemIds)
    {
        if (pinnedItemIds is null)
        {
            return [];
        }

        var normalizedItemIds = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var itemId in pinnedItemIds)
        {
            if (string.IsNullOrWhiteSpace(itemId) || !ValidSidebarItemIds.Contains(itemId) || !seen.Add(itemId))
            {
                continue;
            }

            normalizedItemIds.Add(itemId);
        }

        return normalizedItemIds;
    }

    private static string NormalizeEnumValue(string? value, HashSet<string> validValues, string fallback)
    {
        if (!string.IsNullOrWhiteSpace(value) && validValues.Contains(value))
        {
            return value;
        }

        return fallback;
    }

    private static UserPreferencesDto CreateDefaultPreferences()
    {
        return new UserPreferencesDto
        {
            Version = 1,
            Sidebar = new SidebarPreferencesDto
            {
                Open = true,
                Variant = "inset",
                PinnedItemIds = []
            },
            ChatWidget = new ChatWidgetPreferencesDto
            {
                State = "open",
                Position = "bottom-right"
            }
        };
    }
}