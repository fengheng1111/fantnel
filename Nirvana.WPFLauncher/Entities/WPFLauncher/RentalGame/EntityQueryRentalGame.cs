using System.Text.Json.Serialization;

namespace Nirvana.WPFLauncher.Entities.WPFLauncher.RentalGame;

public class EntityQueryRentalGame {
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("sort_type")]
    public int SortType { get; set; }
}