# NNews.ACL

[![NuGet](https://img.shields.io/nuget/v/NNews.ACL.svg)](https://www.nuget.org/packages/NNews.ACL/)
[![Downloads](https://img.shields.io/nuget/dt/NNews.ACL.svg)](https://www.nuget.org/packages/NNews.ACL/)
[![License](https://img.shields.io/github/license/landim32/NNews.svg)](https://github.com/landim32/NNews/blob/main/LICENSE)

Anti-Corruption Layer (ACL) client library for the NNews content management system. This package provides strongly-typed HTTP clients for consuming the NNews API, including support for articles, categories, tags, images, and AI-powered content generation.

## ?? Installation

```bash
dotnet add package NNews.ACL
```

Or via Package Manager Console:

```powershell
Install-Package NNews.ACL
```

## ?? Features

- **Article Management**: Complete CRUD operations with filtering and search
- **AI Content Generation**: Create and update articles using ChatGPT and DALL-E 3
- **Category Management**: Hierarchical category operations with role-based filtering
- **Tag Management**: Tag CRUD with merging capabilities
- **Image Management**: Upload and retrieve images from storage
- **Type-Safe**: Strongly-typed clients with full IntelliSense support
- **Async/Await**: Modern async API for all operations
- **Cancellation Support**: CancellationToken support for all async operations
- **Dependency Injection Ready**: Easy integration with .NET DI container

## ?? Quick Start

### Setup with Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using NNews.ACL;
using NNews.DTO.Settings;

var services = new ServiceCollection();

// Configure NNews API settings
services.Configure<NNewsSetting>(options =>
{
    options.ApiUrl = "https://api.nnews.example.com";
});

// Register HTTP clients with automatic lifetime management
services.AddHttpClient<IArticleClient, ArticleClient>();
services.AddHttpClient<IArticleAIClient, ArticleAIClient>();
services.AddHttpClient<ICategoryClient, CategoryClient>();
services.AddHttpClient<ITagClient, TagClient>();
services.AddHttpClient<IImageClient, ImageClient>();

var serviceProvider = services.BuildServiceProvider();
```

### Configuration in appsettings.json

```json
{
  "NNews": {
    "ApiUrl": "https://api.nnews.example.com"
  }
}
```

## ?? Available Clients

### 1. ArticleClient

Complete article management with filtering, search, and pagination.

```csharp
using NNews.ACL.Interfaces;
using NNews.DTO;

public class ArticleService
{
    private readonly IArticleClient _articleClient;

    public ArticleService(IArticleClient articleClient)
    {
        _articleClient = articleClient;
    }

    // List all articles with pagination
    public async Task<PagedResult<ArticleInfo>> GetArticlesAsync()
    {
        return await _articleClient.GetAllAsync(
            categoryId: 1,
            page: 1,
            pageSize: 10
        );
    }

    // Filter articles by roles
    public async Task<PagedResult<ArticleInfo>> GetPublicArticlesAsync()
    {
        return await _articleClient.FilterAsync(
            roles: new List<string> { "public" },
            parentId: null,
            page: 1,
            pageSize: 20
        );
    }

    // Get single article
    public async Task<ArticleInfo> GetArticleAsync(int id)
    {
        return await _articleClient.GetByIdAsync(id);
    }

    // Create article
    public async Task<ArticleInfo> CreateArticleAsync(ArticleInfo article)
    {
        return await _articleClient.CreateAsync(article);
    }

    // Update article
    public async Task<ArticleInfo> UpdateArticleAsync(ArticleInfo article)
    {
        return await _articleClient.UpdateAsync(article);
    }
}
```

### 2. ArticleAIClient

AI-powered article creation and updates using ChatGPT and DALL-E 3.

```csharp
using NNews.ACL.Interfaces;
using NNews.DTO;

public class AIContentService
{
    private readonly IArticleAIClient _aiClient;

    public AIContentService(IArticleAIClient aiClient)
    {
        _aiClient = aiClient;
    }

    // Create article with AI
    public async Task<ArticleInfo> CreateWithAIAsync(string prompt, bool generateImage = false)
    {
        return await _aiClient.CreateWithAIAsync(prompt, generateImage);
    }

    // Update article with AI
    public async Task<ArticleInfo> UpdateWithAIAsync(int articleId, string prompt, bool generateImage = false)
    {
        return await _aiClient.UpdateWithAIAsync(articleId, prompt, generateImage);
    }
}
```

#### Example: Create Article with AI

```csharp
var aiClient = serviceProvider.GetRequiredService<IArticleAIClient>();

// Generate complete article with AI
var article = await aiClient.CreateWithAIAsync(
    prompt: "Write a comprehensive article about the future of artificial intelligence in healthcare, including current applications, challenges, and predictions for 2025-2030",
    generateImage: true  // Generate illustrative image with DALL-E 3
);

Console.WriteLine($"Created: {article.Title}");
Console.WriteLine($"Category: {article.Category?.Title}");
Console.WriteLine($"Tags: {string.Join(", ", article.Tags.Select(t => t.Title))}");
Console.WriteLine($"Image: {article.ImageUrl}");
```

#### Example: Update Article with AI

```csharp
// Update existing article with contextual changes
var updated = await aiClient.UpdateWithAIAsync(
    articleId: 123,
    prompt: "Add a new section about the latest GPT-4 medical diagnosis capabilities and update the conclusion to reflect recent breakthroughs in AI-assisted surgery",
    generateImage: false  // Keep existing image
);

Console.WriteLine($"Updated: {updated.Title}");
Console.WriteLine($"Last modified: {updated.UpdatedAt}");
```

### 3. CategoryClient

Hierarchical category management with role-based access control.

```csharp
using NNews.ACL.Interfaces;
using NNews.DTO;

public class CategoryService
{
    private readonly ICategoryClient _categoryClient;

    public CategoryService(ICategoryClient categoryClient)
    {
        _categoryClient = categoryClient;
    }

    // Get all categories
    public async Task<IList<CategoryInfo>> GetAllCategoriesAsync()
    {
        return await _categoryClient.GetAllAsync();
    }

    // Filter categories by roles and parent
    public async Task<IList<CategoryInfo>> GetPublicCategoriesAsync(long? parentId = null)
    {
        return await _categoryClient.FilterAsync(
            roles: new List<string> { "public" },
            parentId: parentId
        );
    }

    // Get category by ID
    public async Task<CategoryInfo> GetCategoryAsync(int id)
    {
        return await _categoryClient.GetByIdAsync(id);
    }

    // Create category
    public async Task<CategoryInfo> CreateCategoryAsync(CategoryInfo category)
    {
        return await _categoryClient.CreateAsync(category);
    }

    // Update category
    public async Task<CategoryInfo> UpdateCategoryAsync(CategoryInfo category)
    {
        return await _categoryClient.UpdateAsync(category);
    }

    // Delete category
    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryClient.DeleteAsync(id);
    }
}
```

### 4. TagClient

Tag management with merging capabilities.

```csharp
using NNews.ACL.Interfaces;
using NNews.DTO;

public class TagService
{
    private readonly ITagClient _tagClient;

    public TagService(ITagClient tagClient)
    {
        _tagClient = tagClient;
    }

    // Get all tags
    public async Task<IList<TagInfo>> GetAllTagsAsync()
    {
        return await _tagClient.GetAllAsync();
    }

    // Get tag by ID
    public async Task<TagInfo> GetTagAsync(int id)
    {
        return await _tagClient.GetByIdAsync(id);
    }

    // Create tag
    public async Task<TagInfo> CreateTagAsync(TagInfo tag)
    {
        return await _tagClient.CreateAsync(tag);
    }

    // Update tag
    public async Task<TagInfo> UpdateTagAsync(TagInfo tag)
    {
        return await _tagClient.UpdateAsync(tag);
    }

    // Delete tag
    public async Task DeleteTagAsync(int id)
    {
        await _tagClient.DeleteAsync(id);
    }

    // Merge tags (move all articles from source to target tag)
    public async Task MergeTagsAsync(long sourceTagId, long targetTagId)
    {
        await _tagClient.MergeTagsAsync(sourceTagId, targetTagId);
    }
}
```

### 5. ImageClient

Image upload functionality for article illustrations.

```csharp
using NNews.ACL.Interfaces;
using Microsoft.AspNetCore.Http;

public class ImageService
{
    private readonly IImageClient _imageClient;

    public ImageService(IImageClient imageClient)
    {
        _imageClient = imageClient;
    }

    // Upload image
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        return await _imageClient.UploadImageAsync(file);
    }
}
```

## ?? Advanced Usage Examples

### Complete Article Creation Flow

```csharp
public class ArticleWorkflow
{
    private readonly IArticleClient _articleClient;
    private readonly ICategoryClient _categoryClient;
    private readonly ITagClient _tagClient;
    private readonly IImageClient _imageClient;

    public ArticleWorkflow(
        IArticleClient articleClient,
        ICategoryClient categoryClient,
        ITagClient tagClient,
        IImageClient imageClient)
    {
        _articleClient = articleClient;
        _categoryClient = categoryClient;
        _tagClient = tagClient;
        _imageClient = imageClient;
    }

    public async Task<ArticleInfo> CreateCompleteArticleAsync(
        string title,
        string content,
        string categorySlug,
        string[] tags,
        IFormFile? image = null)
    {
        // 1. Find or create category
        var categories = await _categoryClient.GetAllAsync();
        var category = categories.FirstOrDefault(c => 
            c.Slug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase));

        if (category == null)
        {
            throw new Exception($"Category '{categorySlug}' not found");
        }

        // 2. Upload image if provided
        string? imageName = null;
        if (image != null)
        {
            imageName = await _imageClient.UploadImageAsync(image);
        }

        // 3. Create article
        var article = new ArticleInfo
        {
            Title = title,
            Content = content,
            CategoryId = category.CategoryId,
            Status = 0,  // Draft
            DateAt = DateTime.UtcNow,
            ImageName = imageName
        };

        // Tag list as comma-separated string
        article.TagList = string.Join(", ", tags);

        return await _articleClient.CreateAsync(article);
    }
}
```

### AI-Powered Content Pipeline

```csharp
public class AIContentPipeline
{
    private readonly IArticleAIClient _aiClient;
    private readonly IArticleClient _articleClient;

    public AIContentPipeline(
        IArticleAIClient aiClient,
        IArticleClient articleClient)
    {
        _aiClient = aiClient;
        _articleClient = articleClient;
    }

    // Generate, review, and publish article
    public async Task<ArticleInfo> GenerateAndPublishAsync(
        string prompt,
        bool autoPublish = false)
    {
        // 1. Generate article with AI
        var draft = await _aiClient.CreateWithAIAsync(
            prompt: prompt,
            generateImage: true
        );

        Console.WriteLine($"Draft created: {draft.Title}");
        Console.WriteLine($"Review at: /articles/{draft.ArticleId}");

        // 2. Optional: Auto-publish
        if (autoPublish)
        {
            draft.Status = 1;  // Published
            return await _articleClient.UpdateAsync(draft);
        }

        return draft;
    }

    // Bulk content generation
    public async Task<List<ArticleInfo>> GenerateBulkContentAsync(
        List<string> prompts,
        bool generateImages = true)
    {
        var articles = new List<ArticleInfo>();

        foreach (var prompt in prompts)
        {
            try
            {
                var article = await _aiClient.CreateWithAIAsync(
                    prompt: prompt,
                    generateImage: generateImages
                );
                articles.Add(article);
                
                // Delay to respect API rate limits
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to generate article: {ex.Message}");
            }
        }

        return articles;
    }

    // Content improvement workflow
    public async Task<ArticleInfo> ImproveArticleAsync(
        int articleId,
        string improvementInstructions)
    {
        // Get current article
        var current = await _articleClient.GetByIdAsync(articleId);
        
        Console.WriteLine($"Current title: {current.Title}");
        Console.WriteLine($"Current tags: {string.Join(", ", current.Tags.Select(t => t.Title))}");

        // Improve with AI
        var improved = await _aiClient.UpdateWithAIAsync(
            articleId: articleId,
            prompt: improvementInstructions,
            generateImage: false  // Keep existing image
        );

        Console.WriteLine($"Improved title: {improved.Title}");
        Console.WriteLine($"New tags: {string.Join(", ", improved.Tags.Select(t => t.Title))}");

        return improved;
    }
}
```

### Search and Filter Articles

```csharp
public class ArticleSearchService
{
    private readonly IArticleClient _articleClient;

    public ArticleSearchService(IArticleClient articleClient)
    {
        _articleClient = articleClient;
    }

    // Get latest published articles
    public async Task<PagedResult<ArticleInfo>> GetLatestPublishedAsync(int count = 10)
    {
        return await _articleClient.FilterAsync(
            roles: new List<string> { "public" },
            parentId: null,
            page: 1,
            pageSize: count
        );
    }

    // Get articles by category
    public async Task<PagedResult<ArticleInfo>> GetByCategoryAsync(
        long categoryId,
        int page = 1,
        int pageSize = 10)
    {
        return await _articleClient.GetAllAsync(
            categoryId: categoryId,
            page: page,
            pageSize: pageSize
        );
    }

    // Get articles with specific roles
    public async Task<PagedResult<ArticleInfo>> GetByRolesAsync(
        List<string> roles,
        int page = 1,
        int pageSize = 20)
    {
        return await _articleClient.FilterAsync(
            roles: roles,
            parentId: null,
            page: page,
            pageSize: pageSize
        );
    }

    // Paginate through all articles
    public async Task<List<ArticleInfo>> GetAllArticlesAsync()
    {
        var allArticles = new List<ArticleInfo>();
        var page = 1;
        var pageSize = 50;
        PagedResult<ArticleInfo> result;

        do
        {
            result = await _articleClient.GetAllAsync(
                categoryId: null,
                page: page,
                pageSize: pageSize
            );

            allArticles.AddRange(result.Items);
            page++;
        }
        while (page <= result.TotalPages);

        return allArticles;
    }
}
```

## ?? Configuration Options

### Custom HTTP Client Configuration

```csharp
services.AddHttpClient<IArticleClient, ArticleClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);  // Long timeout for AI operations
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
})
.AddPolicyHandler(GetRetryPolicy());  // Add Polly retry policy

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

### Authentication Configuration

```csharp
services.AddHttpClient<IArticleClient, ArticleClient>(client =>
{
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "your-api-token");
});
```

## ?? Error Handling

All clients throw appropriate exceptions for different error scenarios:

```csharp
try
{
    var article = await _articleClient.GetByIdAsync(999);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("Article not found");
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    Console.WriteLine("Authentication required");
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
{
    Console.WriteLine($"Invalid request: {ex.Message}");
}
catch (TaskCanceledException)
{
    Console.WriteLine("Request timeout");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## ?? API Response Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 OK | Request successful |
| 201 Created | Resource created successfully |
| 400 Bad Request | Invalid request data |
| 401 Unauthorized | Authentication required |
| 404 Not Found | Resource not found |
| 500 Internal Server Error | Server error |

## ?? Supported Frameworks

- **.NET 8.0+**
- Compatible with:
  - ASP.NET Core Web APIs
  - Blazor Server/WASM
  - Console applications
  - Windows Services
  - Azure Functions
  - AWS Lambda

## ?? Related Packages

- **NNews.DTO**: Data Transfer Objects used by this library
- **NNews.Domain**: Domain entities and business logic
- **NNews.API**: The NNews API itself

## ?? Dependencies

- `NNews.DTO` (>= 2.0.0)
- `Microsoft.Extensions.Options` (>= 8.0.0)
- `System.Net.Http.Json` (>= 8.0.0)

## ?? Interface Summary

```csharp
// Article Management
public interface IArticleClient
{
    Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId, int page, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<ArticleInfo>> FilterAsync(IList<string>? roles, long? parentId, int page, int pageSize, CancellationToken cancellationToken);
    Task<ArticleInfo> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ArticleInfo> CreateAsync(ArticleInfo article, CancellationToken cancellationToken);
    Task<ArticleInfo> UpdateAsync(ArticleInfo article, CancellationToken cancellationToken);
}

// AI-Powered Content
public interface IArticleAIClient
{
    Task<ArticleInfo> CreateWithAIAsync(string prompt, bool generateImage, CancellationToken cancellationToken);
    Task<ArticleInfo> UpdateWithAIAsync(int articleId, string prompt, bool generateImage, CancellationToken cancellationToken);
}

// Category Management
public interface ICategoryClient
{
    Task<IList<CategoryInfo>> GetAllAsync(CancellationToken cancellationToken);
    Task<IList<CategoryInfo>> FilterAsync(IList<string>? roles, long? parentId, CancellationToken cancellationToken);
    Task<CategoryInfo> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CategoryInfo> CreateAsync(CategoryInfo category, CancellationToken cancellationToken);
    Task<CategoryInfo> UpdateAsync(CategoryInfo category, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

// Tag Management
public interface ITagClient
{
    Task<IList<TagInfo>> GetAllAsync(CancellationToken cancellationToken);
    Task<TagInfo> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TagInfo> CreateAsync(TagInfo tag, CancellationToken cancellationToken);
    Task<TagInfo> UpdateAsync(TagInfo tag, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task MergeTagsAsync(long sourceTagId, long targetTagId, CancellationToken cancellationToken);
}

// Image Management
public interface IImageClient
{
    Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken);
}
```

## ?? Contributing

Contributions are welcome! Please read our [Contributing Guidelines](https://github.com/landim32/NNews/blob/main/CONTRIBUTING.md) for details.

## ?? License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/landim32/NNews/blob/main/LICENSE) file for details.

## ?? Security

For security issues, please email security@nnews.example.com instead of using the issue tracker.

## ?? Support

- ?? Email: support@nnews.example.com
- ?? Issues: [GitHub Issues](https://github.com/landim32/NNews/issues)
- ?? Documentation: [Wiki](https://github.com/landim32/NNews/wiki)
- ?? Discussions: [GitHub Discussions](https://github.com/landim32/NNews/discussions)

## ??? Roadmap

- [ ] GraphQL client support
- [ ] Real-time notifications via SignalR
- [ ] Webhook subscription client
- [ ] Built-in caching strategies
- [ ] Offline support with sync
- [ ] Batch operations API

## ?? Changelog

### Version 2.0.0 (Current)
- ? Added AI-powered content clients (`IArticleAIClient`)
- ? Support for ChatGPT content generation
- ? Support for DALL-E 3 image generation
- ?? Updated article DTOs with `tagList` field
- ?? Improved error handling
- ? Performance optimizations

### Version 1.0.0
- ?? Initial release
- ?? Article, category, and tag clients
- ??? Image upload support
- ?? Dependency injection ready

## ?? Acknowledgments

- Built with ?? by the NNews team
- Powered by OpenAI's ChatGPT and DALL-E 3
- Thanks to all our contributors!

---

**Made with ?? for the .NET community**
