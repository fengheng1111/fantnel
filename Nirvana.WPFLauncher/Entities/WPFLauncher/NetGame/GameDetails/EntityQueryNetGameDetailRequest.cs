using System.Text.Json.Serialization;

namespace Nirvana.WPFLauncher.Entities.WPFLauncher.NetGame.GameDetails;

public class EntityQueryNetGameDetailRequest {
    [JsonPropertyName("item_id")]
    public required string ItemId { get; init; }
}