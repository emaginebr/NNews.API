using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NNews.Dtos
{
    public class TagInfo
    {
        [JsonPropertyName("tagId")]
        public long TagId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(120, ErrorMessage = "Title cannot exceed 120 characters")]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "Slug cannot exceed 120 characters")]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; } = string.Empty;

        [JsonPropertyName("articleCount")]
        public int ArticleCount { get; set; }
    }
}
