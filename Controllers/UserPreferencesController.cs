using System.Security.Claims;
using joker_api.Models.Common;
using joker_api.Models.DTOs;
using joker_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace joker_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class UserPreferencesController(
    IUserPreferencesService userPreferencesService,
    IHostEnvironment hostEnvironment,
    IOptions<UserPreferencesOptionsModel> userPreferencesOptions) : ControllerBase
{
    private static readonly string[] UserIdClaimTypes = [ClaimTypes.NameIdentifier, "sub", ClaimTypes.Upn, ClaimTypes.Email];

    private readonly IUserPreferencesService _userPreferencesService = userPreferencesService;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private readonly UserPreferencesOptionsModel _userPreferencesOptions = userPreferencesOptions.Value;

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CommonApiResponseModel<UserPreferencesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonApiResponseModel<UserPreferencesDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CommonApiResponseModel<UserPreferencesDto>>> GetCurrentUserPreferences(CancellationToken cancellationToken = default)
    {
        var userId = ResolveCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(CreateUnauthorizedResponse());
        }

        var preferences = await _userPreferencesService.GetCurrentUserPreferencesAsync(userId, cancellationToken);
        return Ok(new CommonApiResponseModel<UserPreferencesDto>(true, null, preferences));
    }

    [HttpPatch]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CommonApiResponseModel<UserPreferencesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommonApiResponseModel<UserPreferencesDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CommonApiResponseModel<UserPreferencesDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CommonApiResponseModel<UserPreferencesDto>>> UpdateCurrentUserPreferences([FromBody] UserPreferencesDto? preferences, CancellationToken cancellationToken = default)
    {
        var userId = ResolveCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(CreateUnauthorizedResponse());
        }

        if (preferences is null)
        {
            return BadRequest(new CommonApiResponseModel<UserPreferencesDto>(false, "A user preferences payload is required.", null));
        }

        var updatedPreferences = await _userPreferencesService.UpdateCurrentUserPreferencesAsync(userId, preferences, cancellationToken);
        return Ok(new CommonApiResponseModel<UserPreferencesDto>(true, null, updatedPreferences));
    }

    private string? ResolveCurrentUserId()
    {
        foreach (var claimType in UserIdClaimTypes)
        {
            var claimValue = User.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(claimValue))
            {
                return claimValue.Trim();
            }
        }

        if (!string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            return User.Identity.Name.Trim();
        }

        if (_hostEnvironment.IsDevelopment() &&
            _userPreferencesOptions.AllowAnonymousDevelopmentUser &&
            !string.IsNullOrWhiteSpace(_userPreferencesOptions.DevelopmentUserId))
        {
            return _userPreferencesOptions.DevelopmentUserId.Trim();
        }

        return null;
    }

    private static CommonApiResponseModel<UserPreferencesDto> CreateUnauthorizedResponse()
    {
        return new CommonApiResponseModel<UserPreferencesDto>(false, "The current user could not be resolved from the request context.", null);
    }
}