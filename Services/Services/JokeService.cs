

using joker_api.Models.Entities;
using joker_api.Services.Interfaces;

namespace joker_api.Services.Services;

public class JokeService : IJokeService
{
    public async Task<JokeApiResponseEntity> GetRandomJokeAsync()
    {
        // Fetch random dad joke
        /* 
            $ curl -H "Accept: application/json" https://icanhazdadjoke.com/
            {
            "id": "R7UfaahVfFd",
            "joke": "My dog used to chase people on a bike a lot. It got so bad I had to take his bike away.",
            "status": 200
            }
         */

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var response = await httpClient.GetAsync("https://icanhazdadjoke.com/");
        response.EnsureSuccessStatusCode();

        var jokeResponse = await response.Content.ReadFromJsonAsync<JokeApiResponseEntity>() ?? throw new Exception("Failed to fetch joke from API.");

        return jokeResponse;
    }
}