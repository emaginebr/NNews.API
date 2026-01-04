using NNews.DTO;

namespace NNews.Domain.Services.Interfaces
{
    public interface IArticleService
    {
        PagedResult<ArticleInfo> ListAll(long? categoryId, int page, int pageSize);
        PagedResult<ArticleInfo> ListByRoles(IList<string>? roles, int page, int pageSize);
        PagedResult<ArticleInfo> ListByTag(IList<string>? roles, string tagSlug, int page, int pageSize);
        PagedResult<ArticleInfo> ListByCategory(IList<string>? roles, long categoryId, int page, int pageSize);
        PagedResult<ArticleInfo> Search(IList<string>? roles, string keyword, int page, int pageSize);
        ArticleInfo GetById(int articleId);
        ArticleInfo Insert(ArticleInsertedInfo article);
        ArticleInfo Update(ArticleUpdatedInfo article);
        ArticleInfo Schedule(int articleId, DateTime publishDate);
        void PublishScheduledArticles();
    }
}
