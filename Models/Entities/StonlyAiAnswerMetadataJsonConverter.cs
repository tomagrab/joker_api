using System.Text.Json;
using System.Text.Json.Serialization;

namespace joker_api.Models.Entities;

public sealed class StonlyAiAnswerMetadataJsonConverter : JsonConverter<StonlyAiAnswerMetadataBaseEntity>
{
    public override StonlyAiAnswerMetadataBaseEntity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var rawJson = root.GetRawText();

        if (IsGuidedAnswer(root))
        {
            return JsonSerializer.Deserialize<StonlyAiGuidedAnswerEntity>(rawJson, options);
        }

        if (IsClarificationAnswer(root))
        {
            return JsonSerializer.Deserialize<StonlyAiClarificationAnswerMetadataEntity>(rawJson, options);
        }

        return JsonSerializer.Deserialize<StonlyAiAnswerMetadataEntity>(rawJson, options);
    }

    public override void Write(Utf8JsonWriter writer, StonlyAiAnswerMetadataBaseEntity value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case StonlyAiGuidedAnswerEntity guidedAnswer:
                JsonSerializer.Serialize(writer, guidedAnswer, options);
                break;
            case StonlyAiClarificationAnswerMetadataEntity clarificationAnswer:
                JsonSerializer.Serialize(writer, clarificationAnswer, options);
                break;
            case StonlyAiAnswerMetadataEntity answerMetadata:
                JsonSerializer.Serialize(writer, answerMetadata, options);
                break;
            default:
                throw new JsonException($"Unsupported Stonly answer metadata type '{value.GetType().Name}'.");
        }
    }

    private static bool IsGuidedAnswer(JsonElement element)
    {
        return element.TryGetProperty("intentRecognised", out _)
            || element.TryGetProperty("guideId", out _)
            || element.TryGetProperty("stepId", out _)
            || element.TryGetProperty("guideLaunchMode", out _)
            || element.TryGetProperty("detectedIntent", out _)
            || element.TryGetProperty("answerText", out _)
            || element.TryGetProperty("linkText", out _);
    }

    private static bool IsClarificationAnswer(JsonElement element)
    {
        return element.TryGetProperty("clarificationOptions", out _)
            || element.TryGetProperty("clarificationCustomerExplanation", out _)
            || element.TryGetProperty("queryIsAmbiguous", out _)
            || element.TryGetProperty("gptModel", out _);
    }
}