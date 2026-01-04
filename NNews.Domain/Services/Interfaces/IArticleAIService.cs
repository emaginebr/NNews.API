using NNews.DTO;

namespace NNews.Domain.Services.Interfaces
{
    public interface IArticleAIService
    {
        Task<ArticleInfo> InsertWithAI(string prompt, bool generateImage = false);
        Task<ArticleInfo> UpdateWithAI(int articleId, string prompt, bool generateImage = false);
    }
}
