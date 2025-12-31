using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.Dtos;
using NNews.Dtos.Settings;
using System.Net.Http.Json;

namespace NNews.ACL
{
    public class CategoryClient : ICategoryClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Category";

        public CategoryClient(HttpClient httpClient, IOptions<NNewsSetting> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.Value.ApiUrl))
                throw new ArgumentException("ApiUrl cannot be null or empty", nameof(settings));

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
        }

        public async Task<IList<CategoryInfo>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(BaseRoute, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IList<CategoryInfo>>(cancellationToken: cancellationToken);
            return result ?? new List<CategoryInfo>();
        }

        public async Task<IList<CategoryInfo>> FilterAsync(IList<string>? roles = null, long? parentId = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new List<string>();

            if (roles != null && roles.Any())
            {
                queryParams.Add($"roles={string.Join(",", roles)}");
            }

            if (parentId.HasValue)
            {
                queryParams.Add($"parentId={parentId.Value}");
            }

            var query = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
            var response = await _httpClient.GetAsync($"{BaseRoute}/filter{query}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IList<CategoryInfo>>(cancellationToken: cancellationToken);
            return result ?? new List<CategoryInfo>();
        }

        public async Task<CategoryInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CategoryInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize category response");
        }

        public async Task<CategoryInfo> CreateAsync(CategoryInfo category, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, category, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CategoryInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize created category response");
        }

        public async Task<CategoryInfo> UpdateAsync(CategoryInfo category, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseRoute, category, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CategoryInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated category response");
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
