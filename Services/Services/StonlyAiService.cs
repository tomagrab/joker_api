using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using joker_api.Models.Common;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;

namespace joker_api.Services.Services;

public class StonlyAiService(HttpClient httpClient, ILogger<StonlyAiService> logger) : IStonlyAiService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<StonlyAiService> _logger = logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public async Task<CommonApiResponseModel<StonlyAiSearchResponseEntity>> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("ai/search", request, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new CommonApiResponseModel<StonlyAiSearchResponseEntity>
                (
                    false,
                    message: $"Stonly AI search request failed with status code {response.StatusCode}."
                );
            }

            var data = await response.Content.ReadFromJsonAsync<StonlyAiSearchResponseEntity>
            (
                _jsonOptions,
                cancellationToken: cancellationToken
            );

            return data is null
                ? new CommonApiResponseModel<StonlyAiSearchResponseEntity>(false, message: "Stonly AI search returned no data.")
                : new CommonApiResponseModel<StonlyAiSearchResponseEntity>(true, data: data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new CommonApiResponseModel<StonlyAiSearchResponseEntity>(false, message: "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching Stonly AI.");
            return new CommonApiResponseModel<StonlyAiSearchResponseEntity>(false, message: "Stonly search failed.");
        }
    }

    public async Task<CommonApiResponseModel<StonlyAiAnswerResponseEntity>> StonlyAiAnswerAsync(string questionAnswerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"ai/answer?questionAnswerId={Uri.EscapeDataString(questionAnswerId)}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode is not HttpStatusCode.OK and not HttpStatusCode.Accepted)
            {
                return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>
                (
                    false,
                    message: $"Stonly AI answer request failed with status code {response.StatusCode}."
                );
            }

            var data = await response.Content.ReadFromJsonAsync<StonlyAiAnswerResponseEntity>
            (
                _jsonOptions,
                cancellationToken: cancellationToken
            );

            return data is null
                ? new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(false, message: "Stonly AI answer returned no data.")
                : new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(true, data: data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Stonly AI answer operation was canceled.");
            return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(false, message: "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting answer from Stonly AI.");
            return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(false, message: "Stonly AI answer failed.");
        }
    }

    public async Task<CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>> StonlyAiGetAnswerResultStatusAsync(string questionAnswerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var answerResponse = await StonlyAiAnswerAsync(questionAnswerId, cancellationToken);

            if (!answerResponse.Success || answerResponse.Data is null)
            {
                return new CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>
                (
                    false,
                    message: answerResponse.Message ?? "Stonly AI get answer status failed."
                );
            }

            return new CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>(true, data: answerResponse.Data.Status);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Stonly AI get answer status operation was canceled.");
            return new CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>(false, message: "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting answer status from Stonly AI.");
            return new CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>(false, message: "Stonly AI get answer status failed.");
        }
    }

    public async Task<CommonApiResponseModel<StonlyAiAnswerResponseEntity>> StonlyAiSearchAndAnswerAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default)
    {
        try
        {
            var search = await StonlyAiSearchAsync(request, cancellationToken);
            if (!search.Success || search.Data is null)
            {
                return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>
                (
                    false,
                    message: "Stonly AI search failed."
                );
            }

            var questionAnswerId = search.Data.QuestionAnswerId;

            var deadline = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1);
            var delay = TimeSpan.FromSeconds(1);

            while (DateTimeOffset.UtcNow < deadline)
            {
                var answer = await StonlyAiAnswerAsync(questionAnswerId, cancellationToken);
                if (!answer.Success || answer.Data is null)
                {
                    return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>
                    (
                        false,
                        message: answer.Message ?? "Stonly AI answer retrieval failed."
                    );
                }

                if (answer.Data.Status == StonlyAiAnswerResultStatusEntity.COMPLETED)
                {
                    return answer;
                }

                if (answer.Data.Status == StonlyAiAnswerResultStatusEntity.FAILED)
                {
                    return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>
                    (
                        false,
                        message: "Stonly AI answer processing failed."
                    );
                }

                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 5));
            }

            return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>
            (
                false,
                message: "Stonly AI answer processing timed out."
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Stonly AI search and answer operation was canceled.");
            return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(false, message: "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching and answering Stonly AI.");
            return new CommonApiResponseModel<StonlyAiAnswerResponseEntity>(false, message: "Stonly AI search and answer failed.");
        }
    }

    public async Task<CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>> StonlyAiRecommendGuidesAsync(StonlyAiGuideRecommendationRequestEntity request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("ai/recommendGuides", request, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>
                (
                    false,
                    message: $"Stonly AI recommend guides request failed with status code {response.StatusCode}."
                );
            }

            var data = await response.Content.ReadFromJsonAsync<StonlyAiGuideRecommendationResultEntity>
            (
                _jsonOptions,
                cancellationToken: cancellationToken
            );

            return data is null
                ? new CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>(false, message: "Stonly AI recommend guides returned no data.")
                : new CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>(true, data: data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Stonly AI recommend guides operation was canceled.");
            return new CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>(false, message: "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Stonly AI guide recommendations.");
            return new CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>(false, message: "Stonly AI recommend guides failed.");
        }
    }
}
