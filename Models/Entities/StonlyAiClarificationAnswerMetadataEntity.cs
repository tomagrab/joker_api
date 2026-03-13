namespace joker_api.Models.Entities;

public class StonlyAiClarificationAnswerMetadataEntity : StonlyAiAnswerMetadataBaseEntity
{
    public List<string> ClarificationOptions { get; set; } = [];
    public string? ClarificationCustomerExplanation { get; set; }
    public bool? QueryIsAmbiguous { get; set; }
    public string? OriginalQuestion { get; set; }
    public string? Phrase { get; set; }
    public string? GptModel { get; set; }
}