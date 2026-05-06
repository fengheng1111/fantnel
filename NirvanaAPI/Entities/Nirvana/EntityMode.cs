using System.Text.Json.Serialization;

namespace NirvanaAPI.Entities.Nirvana;

public class EntityMode {
    [JsonPropertyName("mode")]
    public required string Mode { get; init; }
}