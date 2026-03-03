using joker_api.Data.Context;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace joker_api.Services.Services;

public class JokeService(ILogger<JokeService> logger, AppDbContext context) : IJokeService
{
    private readonly ILogger<JokeService> _logger = logger;
    private readonly AppDbContext _context = context;

    public async Task<JokeApiResponseEntity> GetRandomJokeFromApiAsync()
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

        /* using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var response = await httpClient.GetAsync("https://icanhazdadjoke.com/");
        response.EnsureSuccessStatusCode();

        var jokeResponse = await response.Content.ReadFromJsonAsync<JokeApiResponseEntity>() ?? throw new Exception("Failed to fetch joke from API.");

        return jokeResponse; */

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await httpClient.GetAsync("https://icanhazdadjoke.com/");

            response.EnsureSuccessStatusCode();

            var jokeResponse = await response.Content.ReadFromJsonAsync<JokeApiResponseEntity>() ?? throw new Exception("Failed to fetch joke from API.");

            if (await JokeExistsAsync(jokeResponse.Id))
            {
                _logger.LogInformation("Joke with ID {JokeId} already exists in the database.", jokeResponse.Id);
                return jokeResponse;
            }

            await AddJokeAsync(jokeResponse);

            return jokeResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching joke from API.");
            throw; // Rethrow the exception to be handled by the global exception handler
        }
    }

    public async Task<IEnumerable<JokeEntity>> GetAllJokesAsync(int? skip, int? take)
    {
        if (take != null && take > 1000)
        {
            take = 1000;
        }

        if (skip == null || skip <= 0)
        {
            skip = 1;
        }

        if (take == null || take <= 0)
        {
            take = 10;
        }

        try
        {
            IEnumerable<JokeEntity> jokes = await
                _context
                .Jokes
                .Skip(((int)skip - 1) * (int)take)
                .Take((int)take)
                .ToListAsync();

            if (jokes == null || !jokes.Any())
            {
                _logger.LogWarning("No jokes found in the database.");
                return [];
            }

            return jokes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching jokes from database.");
            throw;
        }
    }

    public async Task<JokeEntity?> GetJokeByIdAsync(string id)
    {
        try
        {
            var joke = await _context.Jokes.FindAsync(id);
            return joke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching joke by ID from database.");
            throw;
        }
    }

    public async Task<JokeEntity?> UpdateJokeByIdAsync(string id, JokeEntity updatedJoke)
    {
        try
        {
            var existingJoke = await _context.Jokes.FindAsync(id);
            if (existingJoke == null)
            {
                _logger.LogWarning("Joke with ID {JokeId} not found for update.", id);
                return null;
            }

            existingJoke.Joke = updatedJoke.Joke;

            await _context.SaveChangesAsync();
            return existingJoke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating joke by ID in database.");
            throw;
        }
    }

    public async Task<JokeEntity> AddJokeAsync(JokeEntity newJoke)
    {
        try
        {
            _context.Jokes.Add(newJoke);
            await _context.SaveChangesAsync();
            return newJoke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new joke to database.");
            throw;
        }
    }

    public async Task<bool> DeleteJokeByIdAsync(string id)
    {
        try
        {
            var jokeEntity = await _context.Jokes.FindAsync(id);
            if (jokeEntity == null)
            {
                _logger.LogWarning("Joke with ID {JokeId} not found for deletion.", id);
                return false;
            }

            _context.Jokes.Remove(jokeEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting joke by ID from database.");
            throw;
        }
    }

    public async Task<bool> JokeExistsAsync(string id)
    {
        try
        {
            return await _context.Jokes.AnyAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of joke by ID in database.");
            throw;
        }
    }
}