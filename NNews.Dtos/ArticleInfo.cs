using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NNews.Dtos
{
    public class ArticleInfo
    {
        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        [Range(1, long.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("authorId")]
        public long? AuthorId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [Range(0, 4, ErrorMessage = "Status must be 0 (Draft), 1 (Published), 2 (Archived), 3 (Scheduled)")]
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [Required(ErrorMessage = "DateAt is required")]
        [JsonPropertyName("dateAt")]
        public DateTime DateAt { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("category")]
        public CategoryInfo? Category { get; set; }

        [JsonPropertyName("tags")]
        public List<TagInfo> Tags { get; set; } = new();

        [JsonPropertyName("roles")]
        public List<RoleInfo> Roles { get; set; } = new();
    }
}
