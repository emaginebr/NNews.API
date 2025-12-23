using System.Text.Json.Serialization;

namespace NNews.Dtos
{
    public class PagedResult<T>
    {
        [JsonPropertyName("items")]
        public IList<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("has_previous")]
        public bool HasPrevious => Page > 1;

        [JsonPropertyName("has_next")]
        public bool HasNext => Page < TotalPages;
    }
}
