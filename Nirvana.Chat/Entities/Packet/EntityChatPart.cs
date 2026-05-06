using System.Text.Json.Serialization;
using NirvanaAPI.Entities.Nirvana;

namespace Nirvana.Chat.Entities.Packet;

public class EntityChatPart : EntityText {
    [JsonPropertyName("color")]
    public required string Color { get; set; }
}