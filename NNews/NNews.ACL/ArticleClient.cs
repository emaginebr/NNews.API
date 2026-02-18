using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.DTO;
using NNews.DTO.Settings;
using System.Net.Http.Json;

namespace NNews.ACL
{
    public class ArticleClient : IArticleClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Article";

        public ArticleClient(HttpClient httpClient, IOptions<NNewsSetting> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.Value.ApiUrl))
                throw new ArgumentException("ApiUrl cannot be null or empty", nameof(settings));

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
        }

        public async Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (categoryId.HasValue)
            {
                queryParams.Add($"categoryId={categoryId.Value}");
            }

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}?{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ArticleInfo>>(cancellationToken: cancellationToken);
            return result ?? new PagedResult<ArticleInfo>();
        }

        public async Task<PagedResult<ArticleInfo>> ListByCategoryAsync(long categoryId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>
            {
                $"categoryId={categoryId}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}/ListByCategory?{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ArticleInfo>>(cancellationToken: cancellationToken);
            return result ?? new PagedResult<ArticleInfo>();
        }

        public async Task<PagedResult<ArticleInfo>> ListByRolesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}/ListByRoles?{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ArticleInfo>>(cancellationToken: cancellationToken);
            return result ?? new PagedResult<ArticleInfo>();
        }

        public async Task<PagedResult<ArticleInfo>> ListByTagAsync(string tagSlug, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tagSlug))
                throw new ArgumentException("Tag slug cannot be empty", nameof(tagSlug));

            var queryParams = new List<string>
            {
                $"tagSlug={Uri.EscapeDataString(tagSlug)}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}/ListByTag?{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ArticleInfo>>(cancellationToken: cancellationToken);
            return result ?? new PagedResult<ArticleInfo>();
        }

        public async Task<PagedResult<ArticleInfo>> SearchAsync(string keyword, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty", nameof(keyword));

            var queryParams = new List<string>
            {
                $"keyword={Uri.EscapeDataString(keyword)}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}/Search?{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ArticleInfo>>(cancellationToken: cancellationToken);
            return result ?? new PagedResult<ArticleInfo>();
        }

        public async Task<ArticleInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize article response");
        }

        public async Task<ArticleInfo> CreateAsync(ArticleInsertedInfo article, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, article, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize created article response");
        }

        public async Task<ArticleInfo> UpdateAsync(ArticleUpdatedInfo article, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseRoute, article, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated article response");
        }
    }
}
