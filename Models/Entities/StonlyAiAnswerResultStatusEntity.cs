using System.Text.Json.Serialization;

namespace joker_api.Models.Entities;

[JsonConverter(typeof(JsonStringEnumConverter<StonlyAiAnswerResultStatusEntity>))]
public enum StonlyAiAnswerResultStatusEntity
{
    NEW,
    IN_PROGRESS,
    COMPLETED,
    FAILED
}