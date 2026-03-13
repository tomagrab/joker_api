namespace joker_api.Models.Entities;

public class StonlyAiRaisedIssueEntity
{
    public string Issue { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public string? ResolutionSummary { get; set; }
}