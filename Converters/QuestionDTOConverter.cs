using backend.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace backend.Converters;

public class QuestionDtoConverter : JsonConverter<QuestionDTO>
{
    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    public override QuestionDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            throw new JsonException("Missing 'type' discriminator.");

        var typeValue = typeProp.GetString();

        QuestionDTO dto = typeValue switch
        {
            "multiple-choice" => JsonSerializer.Deserialize<MultipleChoiceQuestionDTO>(root.GetRawText(), options),
            "true-false" => JsonSerializer.Deserialize<TrueFalseQuestionDTO>(root.GetRawText(), options),
            "short-answer" => JsonSerializer.Deserialize<ShortAnswerQuestionDTO>(root.GetRawText(), options),
            "fill-in-the-blank" => JsonSerializer.Deserialize<ManualResponseQuestionDTO>(root.GetRawText(), options),
            "constructed-response" => JsonSerializer.Deserialize<ManualResponseQuestionDTO>(root.GetRawText(), options),
            "sorting" => JsonSerializer.Deserialize<SortingQuestionDTO>(root.GetRawText(), options),
            "true-false-thpt" => JsonSerializer.Deserialize<TrueFalseTHPTQuestionDTO>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown question type: {typeValue}")
        };

        return dto;
    }
    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public override void Write(Utf8JsonWriter writer, QuestionDTO value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}