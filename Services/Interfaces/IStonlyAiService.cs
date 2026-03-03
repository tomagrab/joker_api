using joker_api.Models.Entities;

namespace joker_api.Services.Interfaces;

public interface IStonlyAiService
{
    Task<StonlyAiSearchResponseEntity> StonlyAiSearchAsync(StonlyAiSearchRequestEntity request);
    Task<StonlyAiAnswerResponseEntity> StonlyAiAnswerAsync(string response);
}