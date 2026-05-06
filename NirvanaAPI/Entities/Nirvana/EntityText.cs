using System.Text.Json.Serialization;

namespace NirvanaAPI.Entities.Nirvana;

public class EntityText {
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}