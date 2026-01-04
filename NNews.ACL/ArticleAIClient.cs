using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.DTO;
using NNews.DTO.AI;
using NNews.DTO.Settings;
using System.Net.Http.Json;

namespace NNews.ACL
{
    public class ArticleAIClient : IArticleAIClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Article";

        public ArticleAIClient(HttpClient httpClient, IOptions<NNewsSetting> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.Value.ApiUrl))
                throw new ArgumentException("ApiUrl cannot be null or empty", nameof(settings));

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
        }

        public async Task<ArticleInfo> CreateWithAIAsync(string prompt, bool generateImage = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

            var request = new AIArticleRequest
            {
                Prompt = prompt,
                GenerateImage = generateImage
            };

            var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/insertWithAI", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize AI-created article response");
        }

        public async Task<ArticleInfo> UpdateWithAIAsync(int articleId, string prompt, bool generateImage = false, CancellationToken cancellationToken = default)
        {
            if (articleId <= 0)
                throw new ArgumentException("ArticleId must be greater than zero", nameof(articleId));

            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

            var request = new AIArticleRequest
            {
                ArticleId = articleId,
                Prompt = prompt,
                GenerateImage = generateImage
            };

            var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/updateWithAI", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ArticleInfo>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize AI-updated article response");
        }
    }
}
