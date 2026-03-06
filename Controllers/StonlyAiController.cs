using Microsoft.AspNetCore.Mvc;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;
using joker_api.Models.Common;

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
    public async Task<ActionResult<CommonApiResponseModel<StonlyAiSearchResponseEntity>>> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default)
    {
        var response = await _stonlyAiService.StonlyAiSearchAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("answer")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CommonApiResponseModel<StonlyAiAnswerResponseEntity>), 200)]
    public async Task<ActionResult<CommonApiResponseModel<StonlyAiAnswerResponseEntity>>> StonlyAiAnswerAsync([FromQuery] string questionAnswerId, CancellationToken cancellationToken = default)
    {
        var answerResponse = await _stonlyAiService.StonlyAiAnswerAsync(questionAnswerId, cancellationToken);
        return Ok(answerResponse);
    }

    [HttpGet("answer/status")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CommonApiResponseModel<StonlyAiAnswerResultStatusEntity>), 200)]
    public async Task<ActionResult<CommonApiResponseModel<StonlyAiAnswerResultStatusEntity>>> StonlyAIGetAnswerResultStatusAsync([FromQuery] string questionAnswerId, CancellationToken cancellationToken = default)
    {
        var statusResponse = await _stonlyAiService.StonlyAiGetAnswerResultStatusAsync(questionAnswerId, cancellationToken);
        return Ok(statusResponse);
    }

    [HttpPost("search-and-answer")]
    public async Task<ActionResult<CommonApiResponseModel<StonlyAiAnswerResponseEntity>>> StonlyAiSearchAndAnswerAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default)
    {
        var answerResponse = await _stonlyAiService.StonlyAiSearchAndAnswerAsync(request, cancellationToken);
        return Ok(answerResponse);
    }
}