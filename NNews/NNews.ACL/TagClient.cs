using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.DTO;
using NNews.DTO.Settings;
using System.Net.Http.Json;

namespace NNews.ACL
{
    public class TagClient : ITagClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Tag";

        public TagClient(HttpClient httpClient, IOptions<NNewsSetting> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.Value.ApiUrl))
                throw new ArgumentException("ApiUrl cannot be null or empty", nameof(settings));

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
        }

        public async Task<IList<TagInfo>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(BaseRoute, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IList<TagInfo>>(cancellationToken: cancellationToken);
            return result ?? new List<TagInfo>();
        }

        public async Task<IList<TagInfo>> ListByRolesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/ListByRoles", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IList<TagInfo>>(cancellationToken: cancellationToken);
            return result ?? new List<TagInfo>();
        }

        public async Task<TagInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BaseRoute}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TagInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize tag response");
        }

        public async Task<TagInfo> CreateAsync(TagInfo tag, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, tag, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TagInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize created tag response");
        }

        public async Task<TagInfo> UpdateAsync(TagInfo tag, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseRoute, tag, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TagInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize updated tag response");
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task MergeTagsAsync(long sourceTagId, long targetTagId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsync($"{BaseRoute}/merge/{sourceTagId}/{targetTagId}", null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
