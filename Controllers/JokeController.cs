using Microsoft.AspNetCore.Mvc;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;

namespace joker_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JokeController(IJokeService jokeService) : ControllerBase
{
    private readonly IJokeService _jokeService = jokeService;

    [Route("/error")]
    public IActionResult HandleError() =>
        Problem();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JokeEntity>>> GetJokes(int? skip, int? take)
    {
        var jokes = await _jokeService.GetAllJokesAsync(skip, take);
        return Ok(jokes);
    }

    [HttpGet("random")]
    public async Task<ActionResult<JokeApiResponseEntity>> GetRandomJoke()
    {
        var joke = await _jokeService.GetRandomJokeFromApiAsync();
        return Ok(joke);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JokeEntity>> GetJokeEntity(string id)
    {
        var joke = await _jokeService.GetJokeByIdAsync(id);
        if (joke == null)
        {
            return NotFound();
        }
        return Ok(joke);
    }

    public async Task<ActionResult<JokeEntity>> PutJokeEntity(string id, JokeEntity jokeEntity)
    {
        if (id != jokeEntity.Id)
        {
            return BadRequest();
        }

        var updatedJoke = await _jokeService.UpdateJokeByIdAsync(id, jokeEntity);
        if (updatedJoke == null)
        {
            return NotFound();
        }

        return Ok(updatedJoke);
    }

    [HttpPost]
    public async Task<ActionResult<JokeEntity>> PostJokeEntity(JokeEntity jokeEntity)
    {
        var createdJoke = await _jokeService.AddJokeAsync(jokeEntity);
        return CreatedAtAction("GetJokeEntity", new { id = createdJoke.Id }, createdJoke);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJokeEntity(string id)
    {
        var deleted = await _jokeService.DeleteJokeByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

