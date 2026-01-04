using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;
using NNews.DTO.AI;
using NNews.Infra.Interfaces.Repository;
using NTools.ACL.Interfaces;
using NTools.DTO.ChatGPT;
using System.Text.Json;

namespace NNews.Domain.Services
{
    public class ArticleAIService : IArticleAIService
    {
        private readonly IArticleService _articleService;
        private readonly ITagRepository<ITagModel> _tagRepository;
        private readonly ICategoryRepository<ICategoryModel> _categoryRepository;
        private readonly IStringClient _stringClient;
        private readonly IChatGPTClient _chatGPTClient;
        private readonly IFileClient _fileClient;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ArticleAIService> _logger;

        public ArticleAIService(
            IArticleService articleService,
            ITagRepository<ITagModel> tagRepository,
            ICategoryRepository<ICategoryModel> categoryRepository,
            IStringClient stringClient,
            IChatGPTClient chatGPTClient,
            IFileClient fileClient,
            HttpClient httpClient,
            ILogger<ArticleAIService> logger)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _stringClient = stringClient ?? throw new ArgumentNullException(nameof(stringClient));
            _chatGPTClient = chatGPTClient ?? throw new ArgumentNullException(nameof(chatGPTClient));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArticleInfo> InsertWithAI(string prompt, bool generateImage = false)
        {
            _logger.LogTrace("InsertWithAI started with prompt: {Prompt}, generateImage: {GenerateImage}", prompt, generateImage);

            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("InsertWithAI failed: Prompt is empty");
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
            }

            _logger.LogTrace("Fetching categories from repository");
            var categories = _categoryRepository.ListAll()
                .Select(c => new AICategorySummary
                {
                    CategoryId = c.CategoryId,
                    Title = c.Title,
                    ParentId = c.ParentId
                })
                .ToList();
            _logger.LogTrace("Found {Count} categories", categories.Count);

            _logger.LogTrace("Fetching existing tags from repository");
            var existingTags = _tagRepository.ListAll()
                .Select(t => t.Title)
                .ToList();
            _logger.LogTrace("Found {Count} existing tags", existingTags.Count);

            var systemPrompt = @"
You are a professional content writer assistant. Generate a complete article based on the user's request.

IMPORTANT INSTRUCTIONS:
1. Return ONLY a valid JSON object with the exact structure shown below
2. The 'content' field MUST be in HTML format with proper semantic tags (<h2>, <h3>, <p>, <ul>, <li>, <strong>, <em>, etc.)
3. Choose the most appropriate categoryId from the provided list
4. For tagList, use existing tags when possible, but you can create new ones if needed (comma-separated list of tag names)
5. If generateImage is true, provide a detailed imagePrompt in Portuguese describing the desired image
6. Write in a professional, engaging style appropriate for a news/blog article
7. DO NOT include markdown formatting - use HTML only

Response JSON structure:
{
    ""title"": ""Article title here"",
    ""content"": ""<h2>Introduction</h2><p>First paragraph...</p><h2>Main Content</h2><p>More content...</p>"",
    ""categoryId"": 1,
    ""tagList"": ""Technology, Innovation, AI"",
    ""imagePrompt"": ""Detailed image description in Portuguese""
}";

            var userMessage = $@"Generate an article based on this request: {prompt}

Available categories:
{JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true })}

Existing tags you can use (feel free to suggest new ones):
{string.Join(", ", existingTags)}

Remember: Return ONLY the JSON object, no additional text or markdown formatting.";

            _logger.LogTrace("Preparing ChatGPT request with {MessageCount} messages", 2);
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "system", Content = systemPrompt.Trim() },
                new ChatMessage { Role = "user", Content = userMessage }
            };

            _logger.LogTrace("Sending conversation to ChatGPT");
            var chatResponse = await _chatGPTClient.SendConversationAsync(messages);
            _logger.LogTrace("Received ChatGPT response with length: {Length}", chatResponse?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(chatResponse))
            {
                _logger.LogError("ChatGPT returned empty response");
                throw new InvalidOperationException("AI did not return a valid response.");
            }

            _logger.LogTrace("Cleaning ChatGPT response");
            var cleanResponse = chatResponse.Trim();
            if (cleanResponse.StartsWith("```json"))
            {
                _logger.LogTrace("Removing markdown JSON wrapper");
                cleanResponse = cleanResponse.Substring(7);
                if (cleanResponse.EndsWith("```"))
                    cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
                cleanResponse = cleanResponse.Trim();
            }

            _logger.LogTrace("Deserializing AI response JSON");
            _logger.LogDebug("AI Response JSON: {Json}", cleanResponse);
            var aiResponse = JsonSerializer.Deserialize<AIArticleResponse>(cleanResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (aiResponse == null)
            {
                _logger.LogError("Failed to deserialize AI response");
                throw new InvalidOperationException("Failed to deserialize AI response.");
            }

            _logger.LogTrace("AI generated article: Title={Title}, CategoryId={CategoryId}, Tags={Tags}", 
                aiResponse.Title, aiResponse.CategoryId, aiResponse.TagList);

            string? imageName = null;
            if (generateImage && !string.IsNullOrWhiteSpace(aiResponse.ImagePrompt))
            {
                _logger.LogTrace("Starting image generation with prompt: {ImagePrompt}", aiResponse.ImagePrompt);
                imageName = await GenerateAndUploadImageAsync(aiResponse.ImagePrompt);
                _logger.LogTrace("Image generation completed: {ImageName}", imageName ?? "NULL");
            }
            else
            {
                _logger.LogTrace("Skipping image generation");
            }

            _logger.LogTrace("Creating article insert DTO");
            var articleInsert = new ArticleInsertedInfo
            {
                Title = aiResponse.Title,
                Content = aiResponse.Content,
                CategoryId = aiResponse.CategoryId,
                TagList = aiResponse.TagList,
                Status = 0, // Draft
                DateAt = DateTime.UtcNow,
                ImageName = imageName
            };

            _logger.LogTrace("Calling ArticleService.Insert");
            var result = _articleService.Insert(articleInsert);
            _logger.LogInformation("Article created successfully via AI: ArticleId={ArticleId}, Title={Title}", 
                result.ArticleId, result.Title);

            return result;
        }

        public async Task<ArticleInfo> UpdateWithAI(int articleId, string prompt, bool generateImage = false)
        {
            _logger.LogTrace("UpdateWithAI started with articleId: {ArticleId}, prompt: {Prompt}, generateImage: {GenerateImage}", 
                articleId, prompt, generateImage);

            if (articleId <= 0)
            {
                _logger.LogWarning("UpdateWithAI failed: Invalid ArticleId");
                throw new ArgumentException("ArticleId must be greater than zero.", nameof(articleId));
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("UpdateWithAI failed: Prompt is empty");
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
            }

            // Buscar o artigo existente
            _logger.LogTrace("Fetching existing article with ID: {ArticleId}", articleId);
            var existingArticle = _articleService.GetById(articleId);
            
            if (existingArticle == null)
            {
                _logger.LogError("Article not found: {ArticleId}", articleId);
                throw new KeyNotFoundException($"Article with ID {articleId} not found.");
            }

            _logger.LogTrace("Found existing article: Title={Title}, CategoryId={CategoryId}", 
                existingArticle.Title, existingArticle.CategoryId);

            _logger.LogTrace("Fetching categories from repository");
            var categories = _categoryRepository.ListAll()
                .Select(c => new AICategorySummary
                {
                    CategoryId = c.CategoryId,
                    Title = c.Title,
                    ParentId = c.ParentId
                })
                .ToList();
            _logger.LogTrace("Found {Count} categories", categories.Count);

            _logger.LogTrace("Fetching existing tags from repository");
            var existingTags = _tagRepository.ListAll()
                .Select(t => t.Title)
                .ToList();
            _logger.LogTrace("Found {Count} existing tags", existingTags.Count);

            // Preparar dados do artigo atual para o contexto
            var currentArticleData = new
            {
                articleId = existingArticle.ArticleId,
                title = existingArticle.Title,
                content = existingArticle.Content,
                categoryId = existingArticle.CategoryId,
                categoryName = existingArticle.Category?.Title ?? "Unknown",
                tags = existingArticle.Tags?.Select(t => t.Title).ToList() ?? new List<string>(),
                status = existingArticle.Status,
                dateAt = existingArticle.DateAt,
                hasImage = !string.IsNullOrWhiteSpace(existingArticle.ImageName)
            };

            var systemPrompt = @"
You are a professional content editor assistant. Update or improve an existing article based on the user's request.

IMPORTANT INSTRUCTIONS:
1. Return ONLY a valid JSON object with the exact structure shown below
2. The 'content' field MUST be in HTML format with proper semantic tags (<h2>, <h3>, <p>, <ul>, <li>, <strong>, <em>, etc.)
3. You will receive the CURRENT article data - use it as context for your updates
4. Apply the requested changes while maintaining quality and coherence
5. Choose the most appropriate categoryId from the provided list (you can change it if needed)
6. For tagList, use existing tags when possible, but you can create new ones if needed (comma-separated list of tag names)
7. If generateImage is true, provide a detailed imagePrompt in Portuguese describing the desired image
8. Improve and enhance the content while maintaining or improving the original intent
9. DO NOT include markdown formatting - use HTML only
10. ALWAYS return the articleId that was provided

Response JSON structure:
{
    ""articleId"": 123,
    ""title"": ""Updated article title here"",
    ""content"": ""<h2>Introduction</h2><p>First paragraph...</p><h2>Main Content</h2><p>More content...</p>"",
    ""categoryId"": 1,
    ""tagList"": ""Technology, Innovation, AI"",
    ""imagePrompt"": ""Detailed image description in Portuguese""
}";
            var userMessage = $@"Update the following article based on this request: {prompt}

CURRENT ARTICLE DATA:
{JsonSerializer.Serialize(currentArticleData, new JsonSerializerOptions { WriteIndented = true })}

Available categories:
{JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true })}

Existing tags you can use (feel free to suggest new ones):
{string.Join(", ", existingTags)}

REMEMBER: 
- Return the articleId={articleId} in your response
- Return ONLY the JSON object, no additional text or markdown formatting
- Apply the requested changes to the current article content";

            _logger.LogTrace("Preparing ChatGPT request with {MessageCount} messages", 2);
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "system", Content = systemPrompt.Trim() },
                new ChatMessage { Role = "user", Content = userMessage }
            };

            _logger.LogTrace("Sending conversation to ChatGPT");
            var chatResponse = await _chatGPTClient.SendConversationAsync(messages);
            _logger.LogTrace("Received ChatGPT response with length: {Length}", chatResponse?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(chatResponse))
            {
                _logger.LogError("ChatGPT returned empty response");
                throw new InvalidOperationException("AI did not return a valid response.");
            }

            _logger.LogTrace("Cleaning ChatGPT response");
            var cleanResponse = chatResponse.Trim();
            if (cleanResponse.StartsWith("```json"))
            {
                _logger.LogTrace("Removing markdown JSON wrapper");
                cleanResponse = cleanResponse.Substring(7);
                if (cleanResponse.EndsWith("```"))
                    cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
                cleanResponse = cleanResponse.Trim();
            }

            _logger.LogTrace("Deserializing AI response JSON");
            _logger.LogDebug("AI Response JSON: {Json}", cleanResponse);
            
            var aiResponse = JsonSerializer.Deserialize<AIArticleUpdateResponse>(cleanResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (aiResponse == null)
            {
                _logger.LogError("Failed to deserialize AI response");
                throw new InvalidOperationException("Failed to deserialize AI response.");
            }

            // Validar que o ArticleId retornado corresponde ao solicitado
            if (aiResponse.ArticleId != articleId)
            {
                _logger.LogWarning("AI returned different ArticleId. Expected: {Expected}, Received: {Received}. Using expected ID.", 
                    articleId, aiResponse.ArticleId);
                aiResponse.ArticleId = articleId;
            }

            _logger.LogTrace("AI updated article: ArticleId={ArticleId}, Title={Title}, CategoryId={CategoryId}, Tags={Tags}", 
                aiResponse.ArticleId, aiResponse.Title, aiResponse.CategoryId, aiResponse.TagList);

            string? imageName = existingArticle.ImageName;
            if (generateImage && !string.IsNullOrWhiteSpace(aiResponse.ImagePrompt))
            {
                _logger.LogTrace("Starting image generation with prompt: {ImagePrompt}", aiResponse.ImagePrompt);
                var newImageUrl = await GenerateAndUploadImageAsync(aiResponse.ImagePrompt);
                if (!string.IsNullOrWhiteSpace(newImageUrl))
                {
                    imageName = newImageUrl;
                    _logger.LogTrace("Image generation completed: {ImageName}", imageName);
                }
                else
                {
                    _logger.LogWarning("Image generation failed, keeping existing image");
                }
            }
            else
            {
                _logger.LogTrace("Keeping existing image: {ImageName}", imageName ?? "NULL");
            }

            _logger.LogTrace("Creating article update DTO");
            var articleUpdate = new ArticleUpdatedInfo
            {
                ArticleId = articleId,
                Title = aiResponse.Title,
                Content = aiResponse.Content,
                CategoryId = aiResponse.CategoryId,
                TagList = aiResponse.TagList,
                Status = existingArticle.Status, // Manter status atual
                DateAt = existingArticle.DateAt, // Manter data original
                ImageName = imageName
            };

            _logger.LogTrace("Calling ArticleService.Update");
            var result = _articleService.Update(articleUpdate);
            _logger.LogInformation("Article updated successfully via AI: ArticleId={ArticleId}, Title={Title}", 
                result.ArticleId, result.Title);

            return result;
        }

        private async Task<string?> GenerateAndUploadImageAsync(string imagePrompt)
        {
            _logger.LogTrace("GenerateAndUploadImageAsync started with prompt: {Prompt}", imagePrompt);

            // Criar o payload para DALL-E 3
            var imageRequest = new DallERequest
            {
                Prompt = imagePrompt,
                Model = "dall-e-3",
                Size = "1024x1024",
                Quality = "standard",
                Style = "vivid"
            };

            _logger.LogTrace("Calling DALL-E 3 image generation API");
            var imageResponse = await _chatGPTClient.GenerateImageAdvancedAsync(imageRequest);
            _logger.LogTrace("DALL-E 3 response received: HasData={HasData}, DataCount={Count}",
                imageResponse?.Data != null, imageResponse?.Data?.Count ?? 0);

            if (imageResponse == null || imageResponse.Data == null || !imageResponse.Data.Any())
            {
                _logger.LogWarning("No image data returned from DALL-E 3");
                return null;
            }

            var imageUrl = imageResponse.Data.FirstOrDefault()?.Url;
            _logger.LogTrace("Image URL received: {Url}", imageUrl);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                _logger.LogWarning("Empty image URL returned from DALL-E 3");
                return null;
            }

            _logger.LogTrace("Downloading image from URL");
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
            _logger.LogTrace("Image downloaded: {Size} bytes", imageBytes.Length);

            var fileName = $"ai-generated-{Guid.NewGuid()}.png";
            var bucketName = "NNews";
            _logger.LogTrace("Uploading image to bucket: {Bucket}, FileName: {FileName}", bucketName, fileName);

            using var memoryStream = new MemoryStream(imageBytes);
            IFormFile formFile = new FormFileWrapper(memoryStream, fileName, "image/png");

            var uploadedFileName = await _fileClient.UploadFileAsync(bucketName, formFile);
            _logger.LogInformation("Image uploaded successfully: {FileName}", uploadedFileName);

            var uploadedUrl = await _fileClient.GetFileUrlAsync(bucketName, uploadedFileName);
            _logger.LogInformation("Image final URL: {Url}", uploadedUrl);

            return uploadedUrl;
        }
    }

    internal class FormFileWrapper : IFormFile
    {
        private readonly Stream _stream;
        private readonly string _fileName;
        private readonly string _contentType;

        public FormFileWrapper(Stream stream, string fileName, string contentType)
        {
            _stream = stream;
            _fileName = fileName;
            _contentType = contentType;
        }

        public string ContentType => _contentType;
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length => _stream.Length;
        public string Name => "file";
        public string FileName => _fileName;

        public void CopyTo(Stream target) => _stream.CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
            _stream.CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream() => _stream;
    }
}
