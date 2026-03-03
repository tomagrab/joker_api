namespace joker_api.Models.Entities;

public class StonlyAiAnswerResponseEntityMetadataEntity
{
    public string? Language { get; set; }
    public int? AiAgentId { get; set; }
    public string? UserEmail { get; set; }
    public string? WebhookUrl { get; set; }
    public string? KbBaseUrl { get; set; }
}