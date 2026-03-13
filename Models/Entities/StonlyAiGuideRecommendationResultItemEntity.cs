namespace joker_api.Models.Entities;

public class StonlyAiGuideRecommendationResultItemEntity
{
    public string Issue { get; set; } = string.Empty;
    public List<StonlyAiRecommendedGuideEntity> Guides { get; set; } = [];
    public List<StonlyAiRecommendedGuideEntity> GuidesForGeneration { get; set; } = [];
    public bool Verdict { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public bool GenerationVerdict { get; set; }
    public string GenerationExplanation { get; set; } = string.Empty;
}