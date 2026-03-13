namespace joker_api.Models.Entities;

public class StonlyAiAnswerMetadataEntity : StonlyAiAnswerMetadataBaseEntity
{
    public List<StonlyAiAnswerMetadataLinkEntity> Links { get; set; } = new List<StonlyAiAnswerMetadataLinkEntity>();
    public string Language { get; set; } = string.Empty;
    public bool IsFallback { get; set; }
}