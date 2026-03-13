using joker_api.Models.DTOs;

namespace joker_api.Services.Interfaces;

public interface IUserPreferencesService
{
    Task<UserPreferencesDto> GetCurrentUserPreferencesAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserPreferencesDto> UpdateCurrentUserPreferencesAsync(string userId, UserPreferencesDto preferences, CancellationToken cancellationToken = default);
}