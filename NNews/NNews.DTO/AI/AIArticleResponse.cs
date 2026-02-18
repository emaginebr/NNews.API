using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NNews.DTO.AI
{
    public class AIArticleResponse
    {
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
