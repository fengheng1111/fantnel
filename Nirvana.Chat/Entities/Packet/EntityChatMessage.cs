using System.Text.Json.Serialization;
using NirvanaAPI.Entities.Nirvana;

namespace Nirvana.Chat.Entities.Packet;

public class EntityChatMessage : EntityText {
    [JsonPropertyName("extra")]
    public required List<EntityChatPart> Extra { get; set; }
}