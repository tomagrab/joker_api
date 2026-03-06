namespace joker_api.Models.Entities;

public class StonlyAiAnswerResponseEntity
{
    public required string QuestionAnswerId { get; set; }
    public required string ConversationId { get; set; }
    public required string Query { get; set; }
    public required StonlyAiAnswerResponseEntityMetadataEntity QueryMetadata { get; set; }
    public required string Answer { get; set; }
    public required StonlyAiAnswerMetadataEntity AnswerMetadata { get; set; }
    public required StonlyAiAnswerResultStatusEntity Status { get; set; }
}