using joker_api.Models.Entities;

namespace joker_api.Services.Interfaces;

public interface IJokeService
{
    Task<JokeApiResponseEntity> GetRandomJokeFromApiAsync();
    Task<IEnumerable<JokeEntity>> GetAllJokesAsync(int? pageNumber, int? pageSize);
    Task<JokeEntity?> GetJokeByIdAsync(string id);
    Task<JokeEntity?> UpdateJokeByIdAsync(string id, JokeEntity updatedJoke);
    Task<JokeEntity> AddJokeAsync(JokeEntity newJoke);
    Task<bool> DeleteJokeByIdAsync(string id);
    Task<bool> JokeExistsAsync(string id);
}