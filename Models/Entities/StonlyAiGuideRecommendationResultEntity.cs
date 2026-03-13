namespace joker_api.Models.Entities;

public class StonlyAiGuideRecommendationResultEntity
{
    public List<StonlyAiGuideRecommendationResultItemEntity> Results { get; set; } = [];
    public StonlyAiGuideRecommendationDebugEntity? Debug { get; set; }
}