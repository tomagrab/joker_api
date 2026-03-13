namespace joker_api.Models.Entities;

public class StonlyAiAnswerResponseEntity
{
    public string? QuestionAnswerId { get; set; }
    public required string ConversationId { get; set; }
    public required string Query { get; set; }
    public required StonlyAiQueryMetadataEntity QueryMetadata { get; set; }
    public string? Answer { get; set; }
    public StonlyAiAnswerMetadataBaseEntity? AnswerMetadata { get; set; }
    public required StonlyAiAnswerResultStatusEntity Status { get; set; }
}