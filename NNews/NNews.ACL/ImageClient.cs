using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NNews.ACL.Interfaces;
using NNews.DTO.Settings;
using System.Net.Http.Json;

namespace NNews.ACL
{
    public class ImageClient : IImageClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Image";

        public ImageClient(HttpClient httpClient, IOptions<NNewsSetting> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.Value.ApiUrl))
                throw new ArgumentException("ApiUrl cannot be null or empty", nameof(settings));

            _httpClient.BaseAddress = new Uri(settings.Value.ApiUrl);
        }

        public async Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"{BaseRoute}/uploadImage", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<string>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to get image URL from response");
        }
    }
}
