using System.Text.Json.Serialization;

namespace NirvanaAPI.Entities.Nirvana;

public class EntityValue {
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}