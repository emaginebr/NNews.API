# NNews

[![NuGet](https://img.shields.io/nuget/v/NNews.svg)](https://www.nuget.org/packages/NNews/)
[![Downloads](https://img.shields.io/nuget/dt/NNews.svg)](https://www.nuget.org/packages/NNews/)
[![License](https://img.shields.io/github/license/landim32/NNews.API.svg)](https://github.com/landim32/NNews.API/blob/main/LICENSE)

Unified package for the NNews content management system. Contains Data Transfer Objects (DTOs) and Anti-Corruption Layer (ACL) HTTP clients for consuming the NNews API, including support for articles, categories, tags, images, and AI-powered content generation.

## Installation

```bash
dotnet add package NNews
```

Or via Package Manager Console:

```powershell
Install-Package NNews
```

## Features

### DTOs (NNews.DTO)
- **Article DTOs**: Complete models for article creation, updates, and retrieval
- **AI Integration DTOs**: Support for AI-powered content generation with ChatGPT and DALL-E 3
- **Category & Tag DTOs**: Hierarchical category and tag management
- **Pagination Support**: Built-in paged result models
- **Settings DTOs**: Configuration models for NNews API integration
- **JSON Serialization**: Optimized for System.Text.Json with proper property naming

### Clients (NNews.ACL)
- **Article Management**: Complete CRUD operations with filtering and search
- **AI Content Generation**: Create and update articles using ChatGPT and DALL-E 3
- **Category Management**: Hierarchical category operations with role-based filtering
- **Tag Management**: Tag CRUD with merging capabilities
- **Image Management**: Upload and retrieve images from storage
- **Type-Safe**: Strongly-typed clients with full IntelliSense support
- **Async/Await**: Modern async API for all operations
- **Cancellation Support**: CancellationToken support for all async operations
- **Dependency Injection Ready**: Easy integration with .NET DI container

## Core DTOs

### ArticleInfo

```csharp
public class ArticleInfo
{
    public long ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }  // HTML content
    public long CategoryId { get; set; }
    public CategoryInfo? Category { get; set; }
    public IList<TagInfo>? Tags { get; set; }
    public int Status { get; set; }  // 0=Draft, 1=Published, 2=Archived, 3=Scheduled
    public DateTime DateAt { get; set; }
    public string? ImageName { get; set; }
    public IList<RoleInfo>? Roles { get; set; }
}
```

### ArticleInsertedInfo

```csharp
public class ArticleInsertedInfo
{
    public string Title { get; set; }
    public string Content { get; set; }
    public long CategoryId { get; set; }
    public string? TagList { get; set; }  // Comma-separated tags
    public int Status { get; set; }
    public DateTime DateAt { get; set; }
    public string? ImageName { get; set; }
    public List<string>? Roles { get; set; }
}
```

### AI DTOs

```csharp
// Request for AI-powered article creation/updates
public class AIArticleRequest
{
    public long? ArticleId { get; set; }
    public string Prompt { get; set; }
    public bool GenerateImage { get; set; } = false;
}

// AI-generated article response
public class AIArticleResponse
{
    public string Title { get; set; }
    public string Content { get; set; }
    public long CategoryId { get; set; }
    public string TagList { get; set; }
    public string? ImagePrompt { get; set; }  // Portuguese description for DALL-E 3
}
```

### PagedResult<T>

```csharp
public class PagedResult<T>
{
    public IList<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
```

## ACL Clients - Quick Start

### Setup with Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using NNews.ACL;
using NNews.ACL.Interfaces;
using NNews.DTO.Settings;

var services = new ServiceCollection();

// Configure NNews API settings
services.Configure<NNewsSetting>(options =>
{
    options.ApiUrl = "https://api.nnews.example.com";
});

// Register HTTP clients
services.AddHttpClient<IArticleClient, ArticleClient>();
services.AddHttpClient<IArticleAIClient, ArticleAIClient>();
services.AddHttpClient<ICategoryClient, CategoryClient>();
services.AddHttpClient<ITagClient, TagClient>();
services.AddHttpClient<IImageClient, ImageClient>();
```

### ArticleClient Usage

```csharp
using NNews.ACL.Interfaces;
using NNews.DTO;

// List articles with pagination
var articles = await articleClient.GetAllAsync(categoryId: 1, page: 1, pageSize: 10);

// Search articles
var results = await articleClient.SearchAsync("AI technology", page: 1, pageSize: 10);

// Create article
var newArticle = new ArticleInsertedInfo
{
    Title = "Getting Started with AI",
    Content = "<h2>Introduction</h2><p>...</p>",
    CategoryId = 1,
    TagList = "AI, Technology, Innovation",
    Status = 0,  // Draft
    DateAt = DateTime.UtcNow
};
var created = await articleClient.CreateAsync(newArticle);
```

### AI Content Generation

```csharp
using NNews.ACL.Interfaces;

// Create article with AI
var article = await aiClient.CreateWithAIAsync(
    prompt: "Write an article about the future of AI in healthcare",
    generateImage: true  // Generate image with DALL-E 3
);

// Update article with AI
var updated = await aiClient.UpdateWithAIAsync(
    articleId: 123,
    prompt: "Add a section about recent breakthroughs",
    generateImage: false
);
```

### Category & Tag Management

```csharp
// Categories
var categories = await categoryClient.GetAllAsync();
var category = await categoryClient.CreateAsync(new CategoryInfo { Title = "Technology" });
await categoryClient.DeleteAsync(id: 5);

// Tags
var tags = await tagClient.GetAllAsync();
await tagClient.MergeTagsAsync(sourceTagId: 10, targetTagId: 20);
```

## Article Status Codes

| Code | Status | Description |
|------|--------|-------------|
| 0 | Draft | Article is being written |
| 1 | Published | Article is live and visible |
| 2 | Archived | Article is archived |
| 3 | Scheduled | Article scheduled for future publication |

## Interface Summary

```csharp
public interface IArticleClient
{
    Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId, int page, int pageSize, CancellationToken ct);
    Task<PagedResult<ArticleInfo>> ListByCategoryAsync(long categoryId, int page, int pageSize, CancellationToken ct);
    Task<PagedResult<ArticleInfo>> ListByRolesAsync(int page, int pageSize, CancellationToken ct);
    Task<PagedResult<ArticleInfo>> ListByTagAsync(string tagSlug, int page, int pageSize, CancellationToken ct);
    Task<PagedResult<ArticleInfo>> SearchAsync(string keyword, int page, int pageSize, CancellationToken ct);
    Task<ArticleInfo> GetByIdAsync(int id, CancellationToken ct);
    Task<ArticleInfo> CreateAsync(ArticleInsertedInfo article, CancellationToken ct);
    Task<ArticleInfo> UpdateAsync(ArticleUpdatedInfo article, CancellationToken ct);
}

public interface IArticleAIClient
{
    Task<ArticleInfo> CreateWithAIAsync(string prompt, bool generateImage, CancellationToken ct);
    Task<ArticleInfo> UpdateWithAIAsync(int articleId, string prompt, bool generateImage, CancellationToken ct);
}

public interface ICategoryClient
{
    Task<IList<CategoryInfo>> GetAllAsync(CancellationToken ct);
    Task<IList<CategoryInfo>> ListByParentAsync(long? parentId, CancellationToken ct);
    Task<CategoryInfo> GetByIdAsync(int id, CancellationToken ct);
    Task<CategoryInfo> CreateAsync(CategoryInfo category, CancellationToken ct);
    Task<CategoryInfo> UpdateAsync(CategoryInfo category, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public interface ITagClient
{
    Task<IList<TagInfo>> GetAllAsync(CancellationToken ct);
    Task<IList<TagInfo>> ListByRolesAsync(CancellationToken ct);
    Task<TagInfo> GetByIdAsync(int id, CancellationToken ct);
    Task<TagInfo> CreateAsync(TagInfo tag, CancellationToken ct);
    Task<TagInfo> UpdateAsync(TagInfo tag, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task MergeTagsAsync(long sourceTagId, long targetTagId, CancellationToken ct);
}

public interface IImageClient
{
    Task<string> UploadImageAsync(IFormFile file, CancellationToken ct);
}
```

## Supported Frameworks

- **.NET 8.0+**
- Compatible with: ASP.NET Core, Blazor, Console apps, Azure Functions, AWS Lambda

## Dependencies

- `Microsoft.AspNetCore.Http.Features` (>= 5.0.17)
- `Microsoft.Extensions.Configuration.Abstractions` (>= 9.0.0)
- `Microsoft.Extensions.Options` (>= 10.0.0)
- `Microsoft.IdentityModel.Tokens` (>= 8.15.0)

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/landim32/NNews.API/blob/main/LICENSE) file for details.

---

**Made with love for the .NET community**
