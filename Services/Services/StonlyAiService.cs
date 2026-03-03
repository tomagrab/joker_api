using System.Text.Json;
using System.Text.Json.Serialization;
using joker_api.Models.Entities;
using joker_api.Services.Interfaces;

namespace joker_api.Services.Services;

public class StonlyAiService(ILogger<StonlyAiService> logger) : IStonlyAiService
{
    private readonly ILogger<StonlyAiService> _logger = logger;

    public async Task<StonlyAiSearchResponseEntity> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic dG9tbXkuZ3JhYm93c2tpQHZlbG9jaXRvci5jb206M29acUgzUkVBdW5Meks5M3pNekZtVWVWSDRmSkRRc3VHNjB4");

            // log the headers for debugging
            foreach (var header in httpClient.DefaultRequestHeaders)
            {
                _logger.LogInformation("Header: {Key} = {Value}", header.Key, string.Join(", ", header.Value));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var response = await httpClient.PostAsJsonAsync("https://public.stonly.com/api/v3/ai/search", request, options);

            // log the response status code for debugging
            _logger.LogInformation("Response: {Response}", response);

            var rawBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Raw Response Body: {Body}", rawBody);

            var stonlyAiSearchResponse = await response.Content.ReadFromJsonAsync<StonlyAiSearchResponseEntity>(options) ?? throw new Exception("Failed to search Stonly AI.");

            // log the response content for debugging
            _logger.LogInformation("Response Content: {Content}", JsonSerializer.Serialize(stonlyAiSearchResponse, options));

            return stonlyAiSearchResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching Stonly AI.");
            throw; // Rethrow the exception to be handled by the global exception handler
        }
    }

    public async Task<StonlyAiAnswerResponseEntity> StonlyAiAnswerAsync(string questionAnswerId)
    {
        try
        {
            // Example cURL:
            /*
                curl -X GET "https://public.stonly.com/api/v3/ai/answer?questionAnswerId=c13b5bc0-ce71-4b32-bf67-bd589e1ec8e9" -H "accept: application/json" -H "Authorization: Basic dG9tbXkuZ3JhYm93c2tpQHZlbG9jaXRvci5jb206M29acUgzUkVBdW5Meks5M3pNekZtVWVWSDRmSkRRc3VHNjB4"
            */

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic dG9tbXkuZ3JhYm93c2tpQHZlbG9jaXRvci5jb206M29acUgzUkVBdW5Meks5M3pNekZtVWVWSDRmSkRRc3VHNjB4");
            var url = $"https://public.stonly.com/api/v3/ai/answer?questionAnswerId={questionAnswerId}";
            var httpResponse = await httpClient.GetAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var rawBody = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Raw Response Body: {Body}", rawBody);
            var stonlyAiAnswerResponse = await httpResponse.Content.ReadFromJsonAsync<StonlyAiAnswerResponseEntity>(options) ?? throw new Exception("Failed to get answer from Stonly AI.");

            return stonlyAiAnswerResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting answer from Stonly AI.");
            throw; // Rethrow the exception to be handled by the global exception handler
        }
    }
}
