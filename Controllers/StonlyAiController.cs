using Microsoft.AspNetCore.Mvc;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;

namespace joker_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StonlyAiController(IStonlyAiService stonlyAiService) : ControllerBase
{
    private readonly IStonlyAiService _stonlyAiService = stonlyAiService;

    [Route("/error")]
    public IActionResult HandleError() =>
        Problem();

    [HttpPost("search")]
    public async Task<ActionResult<StonlyAiSearchResponseEntity>> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request)
    {
        var response = await _stonlyAiService.StonlyAiSearchAsync(request);
        return Ok(response);
    }

    [HttpGet("answer")]
    public async Task<ActionResult<StonlyAiAnswerResponseEntity>> StonlyAiAnswerAsync([FromQuery] string questionAnswerId)
    {
        var answerResponse = await _stonlyAiService.StonlyAiAnswerAsync(questionAnswerId);
        return Ok(answerResponse);
    }
}