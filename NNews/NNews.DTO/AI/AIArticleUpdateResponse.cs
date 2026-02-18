using System.Text.Json.Serialization;

namespace NNews.DTO.AI
{
    public class AIArticleUpdateResponse
    {
        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("tagList")]
        public string TagList { get; set; } = string.Empty;

        [JsonPropertyName("imagePrompt")]
        public string? ImagePrompt { get; set; }
    }
}
