using System.Text.Json.Serialization;

namespace NirvanaAPI.Entities.Nirvana;

public class EntityMessage {
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}