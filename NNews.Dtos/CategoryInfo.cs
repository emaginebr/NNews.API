using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NNews.Dtos
{
    public class CategoryInfo
    {
        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("parentId")]
        public long? ParentId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(240, ErrorMessage = "Title cannot exceed 240 characters")]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("articleCount")]
        public int ArticleCount { get; set; }
    }
}
