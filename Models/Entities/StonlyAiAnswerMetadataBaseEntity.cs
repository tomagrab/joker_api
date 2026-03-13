using System.Text.Json.Serialization;

namespace joker_api.Models.Entities;

[JsonConverter(typeof(StonlyAiAnswerMetadataJsonConverter))]
public abstract class StonlyAiAnswerMetadataBaseEntity
{
}