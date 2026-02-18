using System.Text.Json.Serialization;

namespace NNews.DTO
{
    public class RoleInfo
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
