using NNews.DTO;

namespace NNews.ACL.Interfaces
{
    public interface IArticleAIClient
    {
        Task<ArticleInfo> CreateWithAIAsync(string prompt, bool generateImage = false, CancellationToken cancellationToken = default);
        Task<ArticleInfo> UpdateWithAIAsync(int articleId, string prompt, bool generateImage = false, CancellationToken cancellationToken = default);
    }
}
