using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NNews.DTO
{
    public class ArticleInsertedInfo
    {
        [Required(ErrorMessage = "CategoryId is required")]
        [Range(1, long.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("authorId")]
        public long? AuthorId { get; set; }

        [StringLength(560, ErrorMessage = "Image name cannot exceed 560 characters")]
        [JsonPropertyName("imageName")]
        public string? ImageName { get; set; }

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

        [JsonPropertyName("tagList")]
        public string TagList { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new();
    }
}
