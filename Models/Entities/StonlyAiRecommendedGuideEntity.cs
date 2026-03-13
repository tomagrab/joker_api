namespace joker_api.Models.Entities;

public class StonlyAiRecommendedGuideEntity
{
    public string GuideId { get; set; } = string.Empty;
    public string GuideType { get; set; } = string.Empty;
    public long StepId { get; set; }
    public string StepTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Content { get; set; }
    public float Score { get; set; }
}