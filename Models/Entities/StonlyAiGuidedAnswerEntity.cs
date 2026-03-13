namespace joker_api.Models.Entities;

public class StonlyAiGuidedAnswerEntity : StonlyAiAnswerMetadataBaseEntity
{
    public int IntentRecognised { get; set; }
    public string GuideId { get; set; } = string.Empty;
    public string StepId { get; set; } = string.Empty;
    public string GuideLaunchMode { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string OriginalQuestion { get; set; } = string.Empty;
    public string Phrase { get; set; } = string.Empty;
    public string DetectedIntent { get; set; } = string.Empty;
    public string AnswerText { get; set; } = string.Empty;
    public string LinkText { get; set; } = string.Empty;
}