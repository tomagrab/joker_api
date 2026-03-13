namespace joker_api.Models.Entities;

public class StonlyAiGuideRecommendationRequestEntity
{
    public StonlyAiGuideRecommendationInputEntity Issues { get; set; } = new();
    public int KbId { get; set; }
    public List<string> TagFilter { get; set; } = [];
    public bool IncludeDebug { get; set; }
}