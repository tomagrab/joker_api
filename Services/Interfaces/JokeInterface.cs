using joker_api.Models.Entities;

namespace joker_api.Services.Interfaces;

public interface IJokeService
{
    Task<JokeApiResponseEntity> GetRandomJokeAsync();
}