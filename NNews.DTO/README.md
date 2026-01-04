# NNews.DTO

[![NuGet](https://img.shields.io/nuget/v/NNews.DTO.svg)](https://www.nuget.org/packages/NNews.DTO/)
[![Downloads](https://img.shields.io/nuget/dt/NNews.DTO.svg)](https://www.nuget.org/packages/NNews.DTO/)
[![License](https://img.shields.io/github/license/landim32/NNews.svg)](https://github.com/landim32/NNews/blob/main/LICENSE)

Data Transfer Objects (DTOs) for the NNews content management system. This package contains all the data models used for API communication, including article management, categories, tags, and AI-powered content generation.

## ?? Installation

```bash
dotnet add package NNews.DTO
```

Or via Package Manager Console:

```powershell
Install-Package NNews.DTO
```

## ?? Features

- **Article DTOs**: Complete models for article creation, updates, and retrieval
- **AI Integration DTOs**: Support for AI-powered content generation with ChatGPT and DALL-E 3
- **Category & Tag DTOs**: Hierarchical category and tag management
- **Pagination Support**: Built-in paged result models
- **Settings DTOs**: Configuration models for NNews API integration
- **JSON Serialization**: Optimized for System.Text.Json with proper property naming

## ?? Core DTOs

### Articles

#### ArticleInfo
Complete article information returned from API:

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
    public string? ImageUrl { get; set; }
    public IList<RoleInfo>? Roles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### ArticleInsertedInfo
Model for creating new articles:

```csharp
public class ArticleInsertedInfo
{
    public string Title { get; set; }
    public string Content { get; set; }  // HTML format
    public long CategoryId { get; set; }
    public string? TagList { get; set; }  // Comma-separated tags: "AI, Technology, Innovation"
    public int Status { get; set; }
    public DateTime DateAt { get; set; }
    public string? ImageName { get; set; }
    public List<string>? Roles { get; set; }
}
```

#### ArticleUpdatedInfo
Model for updating existing articles:

```csharp
public class ArticleUpdatedInfo
{
    public long ArticleId { get; set; }
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

### AI-Powered Content Generation

#### AIArticleRequest
Request model for AI-powered article creation and updates:

```csharp
public class AIArticleRequest
{
    [JsonPropertyName("articleId")]
    public long? ArticleId { get; set; }  // Required for updates

    [Required(ErrorMessage = "Prompt is required")]
    [StringLength(2000, MinimumLength = 10)]
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("generateImage")]
    public bool GenerateImage { get; set; } = false;

    [JsonPropertyName("categoryId")]
    public long? CategoryId { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();

    [Range(0, 3)]
    [JsonPropertyName("status")]
    public int Status { get; set; } = 0;
}
```

#### AIArticleResponse
AI-generated article content:

```csharp
public class AIArticleResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }  // HTML format

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }

    [JsonPropertyName("tagList")]
    public string TagList { get; set; }

    [JsonPropertyName("imagePrompt")]
    public string? ImagePrompt { get; set; }  // Portuguese description for DALL-E 3
}
```

#### AIArticleUpdateResponse
AI response for article updates (includes ArticleId):

```csharp
public class AIArticleUpdateResponse
{
    [JsonPropertyName("articleId")]
    public long ArticleId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }

    [JsonPropertyName("tagList")]
    public string TagList { get; set; }

    [JsonPropertyName("imagePrompt")]
    public string? ImagePrompt { get; set; }
}
```

### Categories

#### CategoryInfo
```csharp
public class CategoryInfo
{
    public long CategoryId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public string Slug { get; set; }
    public IList<RoleInfo>? Roles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### Tags

#### TagInfo
```csharp
public class TagInfo
{
    public long TagId { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### Pagination

#### PagedResult<T>
Generic paged result wrapper:

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

## ?? Usage Examples

### Creating an Article

```csharp
using NNews.DTO;

var newArticle = new ArticleInsertedInfo
{
    Title = "Getting Started with AI",
    Content = "<h2>Introduction</h2><p>Artificial Intelligence is transforming...</p>",
    CategoryId = 1,
    TagList = "AI, Technology, Machine Learning, Innovation",
    Status = 0,  // Draft
    DateAt = DateTime.UtcNow,
    ImageName = "ai-intro.png",
    Roles = new List<string> { "public" }
};
```

### Creating Article with AI

```csharp
using NNews.DTO.AI;

var aiRequest = new AIArticleRequest
{
    Prompt = "Write a comprehensive article about the latest AI trends in 2024, including ChatGPT-4, Gemini, and practical applications in various industries",
    GenerateImage = true,  // Generate image with DALL-E 3
    CategoryId = 1,
    Roles = new List<string> { "public" },
    Status = 0  // Draft
};

// The AI will:
// 1. Generate HTML content with proper structure
// 2. Suggest relevant tags automatically
// 3. Generate an illustrative image (if requested)
// 4. Select or confirm the appropriate category
```

### Updating Article with AI

```csharp
var updateRequest = new AIArticleRequest
{
    ArticleId = 123,  // Required for updates
    Prompt = "Add a new section about the latest ChatGPT-4 updates released in January 2024 and improve the introduction to be more engaging",
    GenerateImage = true  // Generate new image or keep existing
};

// The AI receives the current article content as context
// and applies the requested changes intelligently
```

### Working with Paged Results

```csharp
PagedResult<ArticleInfo> articles = await client.GetArticlesAsync(
    categoryId: 1,
    page: 1,
    pageSize: 10
);

Console.WriteLine($"Total articles: {articles.TotalCount}");
Console.WriteLine($"Total pages: {articles.TotalPages}");

foreach (var article in articles.Items)
{
    Console.WriteLine($"- {article.Title}");
    Console.WriteLine($"  Tags: {string.Join(", ", article.Tags.Select(t => t.Title))}");
}
```

### Processing Tags

```csharp
// Tags are comma-separated strings in requests
var articleUpdate = new ArticleUpdatedInfo
{
    ArticleId = 123,
    TagList = "AI, Machine Learning, Deep Learning, Neural Networks",
    // ... other properties
};

// Tags are returned as objects in responses
var article = await client.GetArticleByIdAsync(123);
var tagTitles = article.Tags.Select(t => t.Title).ToList();
// ["AI", "Machine Learning", "Deep Learning", "Neural Networks"]

// Convert back to string for editing
var tagString = string.Join(", ", tagTitles);
```

## ?? Configuration

### NNews API Settings

```csharp
using NNews.DTO.Settings;

var settings = new NNewsSetting
{
    ApiUrl = "https://api.nnews.example.com"
};
```

## ?? Article Status Codes

| Code | Status | Description |
|------|--------|-------------|
| 0 | Draft | Article is being written |
| 1 | Published | Article is live and visible |
| 2 | Archived | Article is archived |
| 3 | Scheduled | Article scheduled for future publication |

## ?? AI Features

### Content Generation
- **ChatGPT Integration**: Generate professional article content in HTML format
- **Context-Aware Updates**: AI receives current article content when updating
- **Automatic Tag Suggestion**: AI suggests relevant tags based on content
- **Category Selection**: AI selects appropriate category or uses provided one

### Image Generation
- **DALL-E 3 Integration**: Generate illustrative images for articles
- **Portuguese Prompts**: Image generation uses Portuguese descriptions
- **Automatic Upload**: Generated images are automatically uploaded and linked
- **Optional Generation**: Choose whether to generate images or use existing ones

## ?? Supported Frameworks

- **.NET 8.0+**
- Compatible with:
  - ASP.NET Core Web APIs
  - Blazor applications
  - Console applications
  - Azure Functions
  - AWS Lambda

## ?? Related Packages

- **NNews.ACL**: Client library for consuming NNews API
- **NNews.Domain**: Domain entities and business logic
- **NNews.Infra**: Infrastructure and data access layer

## ?? Dependencies

- `System.ComponentModel.Annotations` (>= 5.0.0)
- `System.Text.Json` (>= 8.0.0)

## ?? Validation

DTOs include built-in validation attributes:

```csharp
[Required(ErrorMessage = "Title is required")]
[StringLength(200, MinimumLength = 3)]
public string Title { get; set; }

[StringLength(2000, MinimumLength = 10, 
    ErrorMessage = "Prompt must be between 10 and 2000 characters")]
public string Prompt { get; set; }

[Range(0, 3, ErrorMessage = "Status must be 0-3")]
public int Status { get; set; }
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

- [ ] GraphQL schema support
- [ ] Localization support for multiple languages
- [ ] Advanced validation rules
- [ ] Webhook event DTOs
- [ ] Real-time notification DTOs

## ?? Changelog

### Version 2.0.0 (Current)
- ? Added AI-powered content generation DTOs
- ? Added `AIArticleRequest`, `AIArticleResponse`, `AIArticleUpdateResponse`
- ? Added `tagList` field to article DTOs (comma-separated string)
- ?? Enhanced validation attributes
- ?? Improved JSON serialization with System.Text.Json

### Version 1.0.0
- ?? Initial release
- ?? Core article, category, and tag DTOs
- ?? Pagination support
- ?? Settings DTOs

## ?? Acknowledgments

- Built with ?? by the NNews team
- Powered by OpenAI's ChatGPT and DALL-E 3 for AI features
- Thanks to all our contributors!

---

**Made with ?? for the .NET community**
