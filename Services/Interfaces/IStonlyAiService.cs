using joker_api.Models.Common;
using joker_api.Models.Entities;

namespace joker_api.Services.Interfaces;

public interface IStonlyAiService
{
    Task<CommonApiResponseModel<StonlyAiSearchResponseEntity>> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default);
    Task<CommonApiResponseModel<StonlyAiAnswerResponseEntity>> StonlyAiAnswerAsync(string questionAnswerId, CancellationToken cancellationToken = default);
    Task<CommonApiResponseModel<StonlyAiAnswerResultStatusEntity?>> StonlyAiGetAnswerResultStatusAsync(string questionAnswerId, CancellationToken cancellationToken = default);
    Task<CommonApiResponseModel<StonlyAiAnswerResponseEntity>> StonlyAiSearchAndAnswerAsync(StonlyAiSearchRequestEntity request, CancellationToken cancellationToken = default);
    Task<CommonApiResponseModel<StonlyAiGuideRecommendationResultEntity>> StonlyAiRecommendGuidesAsync(StonlyAiGuideRecommendationRequestEntity request, CancellationToken cancellationToken = default);
}