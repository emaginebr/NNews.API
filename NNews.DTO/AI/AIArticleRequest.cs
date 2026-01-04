using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NNews.DTO.AI
{
    public class AIArticleRequest
    {
        [JsonPropertyName("articleId")]
        public long? ArticleId { get; set; }

        [Required(ErrorMessage = "Prompt is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Prompt must be between 10 and 2000 characters")]
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("generateImage")]
        public bool GenerateImage { get; set; } = false;
    }
}
