using System.Text.Json.Serialization;

namespace NNews.Dtos
{
    public class PagedResult<T>
    {
        [JsonPropertyName("items")]
        public IList<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("hasPrevious")]
        public bool HasPrevious => Page > 1;

        [JsonPropertyName("hasNext")]
        public bool HasNext => Page < TotalPages;
    }
}
