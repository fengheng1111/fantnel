using System.Text.Json.Serialization;

namespace Nirvana.WPFLauncher.Entities.Pc4399;

public class EntityC4399UniAuth {
    [JsonPropertyName("data")]
    public EntityC4399UniAuthData Data { get; init; } = new();
}