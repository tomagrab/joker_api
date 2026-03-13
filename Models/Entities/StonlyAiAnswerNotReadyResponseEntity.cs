namespace joker_api.Models.Entities;

public class StonlyAiAnswerNotReadyResponseEntity
{
    public required string ConversationId { get; set; }
    public required string Query { get; set; }
    public required StonlyAiQueryMetadataEntity QueryMetadata { get; set; }
    public required StonlyAiAnswerResultStatusEntity Status { get; set; }
}