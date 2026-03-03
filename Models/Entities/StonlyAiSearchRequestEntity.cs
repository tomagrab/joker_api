namespace joker_api.Models.Entities;

public class StonlyAiSearchRequestEntity
{
    public string Query { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int AiAgentId { get; set; }
    public Guid? ConversationId { get; set; }
    public string? KbBaseUrl { get; set; }
    public string? UserEmail { get; set; }
    public string? WebhookUrl { get; set; }
}