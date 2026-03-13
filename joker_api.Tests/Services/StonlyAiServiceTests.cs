using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using joker_api.Models.Entities;
using joker_api.Services.Services;

namespace joker_api.Tests.Services;

public class StonlyAiServiceTests
{
    [Fact]
    public async Task RecommendGuidesAsync_ReturnsRecommendations_WhenResponseIsSuccessful()
    {
        const string responseJson = """
        {
          "results": [
            {
              "issue": "Trigger is not showing up despite enabling the Stonly extension",
              "guides": [
                {
                  "guideId": "ULCR6fsl0C",
                  "guideType": "GUIDE",
                  "stepId": 3864161,
                  "stepTitle": "Type of trigger",
                  "title": "Stonly Widget troubleshooting guide",
                  "type": "keyword",
                  "content": "Check the trigger configuration.",
                  "score": 0.85
                }
              ],
              "guidesForGeneration": [],
              "verdict": true,
              "explanation": "The guides provide sufficient information to address the issue",
              "generationVerdict": true,
              "generationExplanation": "The guides contain enough context for generating a comprehensive answer"
            }
          ],
          "debug": {
            "queryExpansionTime": 1234,
            "searchTime": 567,
            "issueCount": 2
          }
        }
        """;

        var service = CreateService(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://public.stonly.com/api/v3/ai/recommendGuides", request.RequestUri?.ToString());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        });

        var result = await service.StonlyAiRecommendGuidesAsync(new StonlyAiGuideRecommendationRequestEntity
        {
            KbId = 123,
            Issues = new StonlyAiGuideRecommendationInputEntity
            {
                RaisedIssues =
                [
                    new StonlyAiRaisedIssueEntity
                    {
                        Issue = "Trigger is not showing up despite enabling the Stonly extension"
                    }
                ]
            },
            IncludeDebug = true
        });

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Results);
        Assert.Equal("ULCR6fsl0C", result.Data.Results[0].Guides[0].GuideId);
        Assert.Equal(2, result.Data.Debug?.IssueCount);
    }

    [Fact]
    public async Task RecommendGuidesAsync_ReturnsFailure_WhenRequestFails()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await service.StonlyAiRecommendGuidesAsync(new StonlyAiGuideRecommendationRequestEntity
        {
            KbId = 123,
            Issues = new StonlyAiGuideRecommendationInputEntity
            {
                RaisedIssues =
                [
                    new StonlyAiRaisedIssueEntity
                    {
                        Issue = "Knowledge base missing"
                    }
                ]
            }
        });

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Stonly AI recommend guides request failed with status code NotFound.", result.Message);
    }

    [Fact]
    public async Task GetAnswerResultStatusAsync_ReturnsFailure_WhenAnswerRequestFails()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await service.StonlyAiGetAnswerResultStatusAsync("qa-123");

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Stonly AI answer request failed with status code InternalServerError.", result.Message);
    }

    [Fact]
    public async Task GetAnswerResultStatusAsync_ReturnsAcceptedStatus_WhenAnswerIsInProgress()
    {
        const string responseJson = """
        {
          "conversationId": "928b59ad-f24d-44b2-933f-4c88fbcc75c7",
          "query": "What is Stonly?",
          "queryMetadata": {
            "language": "en",
            "aiAgentId": 736
          },
          "status": "IN_PROGRESS"
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var result = await service.StonlyAiGetAnswerResultStatusAsync("qa-123");

        Assert.True(result.Success);
        Assert.Equal(StonlyAiAnswerResultStatusEntity.IN_PROGRESS, result.Data);
        Assert.Null(result.Message);
    }

    private static StonlyAiService CreateService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var handler = new StubHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://public.stonly.com/api/v3/")
        };

        return new StonlyAiService(httpClient, NullLogger<StonlyAiService>.Instance);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responseFactory(request));
        }
    }
}