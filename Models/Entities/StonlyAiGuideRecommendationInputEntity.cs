namespace joker_api.Models.Entities;

public class StonlyAiGuideRecommendationInputEntity
{
    public List<StonlyAiRaisedIssueEntity> RaisedIssues { get; set; } = [];
    public List<string> PriorUserActions { get; set; } = [];
    public List<string> ErrorMessages { get; set; } = [];
    public List<string> TechnicalDetails { get; set; } = [];
}