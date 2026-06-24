using System.Text.Json.Serialization;
// Data Contract
public record MeetingAnalysis(
    [property: JsonPropertyName("topic")] string Topic,
    [property: JsonPropertyName("actionItems")] string[] ActionItems,
    [property: JsonPropertyName("sentiment")] string Sentiment
);