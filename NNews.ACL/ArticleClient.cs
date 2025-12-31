using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.Dtos;
using NNews.Dtos.Settings;
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

        public async Task<PagedResult<ArticleInfo>> FilterAsync(IList<string>? roles = null, long? parentId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (roles != null && roles.Any())
            {
                queryParams.Add($"roles={string.Join(",", roles)}");
            }

            if (parentId.HasValue)
            {
                queryParams.Add($"parentId={parentId.Value}");
            }

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{BaseRoute}/filter?{query}", cancellationToken);
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

        public async Task<ArticleInfo> CreateAsync(ArticleInfo article, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, article, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize created article response");
        }

        public async Task<ArticleInfo> UpdateAsync(ArticleInfo article, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseRoute, article, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated article response");
        }
    }
}
