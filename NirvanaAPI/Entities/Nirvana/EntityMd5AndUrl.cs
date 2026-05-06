using System.Text.Json.Serialization;

namespace NirvanaAPI.Entities.Nirvana;

public class EntityMd5AndUrl {
    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}