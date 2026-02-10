using Microsoft.AspNetCore.Mvc;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;
using joker_api.Services.Services;


namespace joker_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JokeController(IJokeService jokeService) : ControllerBase
{
    [HttpGet("random")]
    public async Task<ActionResult<JokeApiResponseEntity>> GetRandomJoke()
    {
        try
        {
            var joke = await jokeService.GetRandomJokeAsync();
            return Ok(joke);
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            return StatusCode(500, new { error = "Failed to fetch joke", details = ex.Message });
        }
    }
}