using NNews.Dtos;

namespace NNews.Domain.Services.Interfaces
{
    public interface IArticleService
    {
        PagedResult<ArticleInfo> ListAll(long? categoryId, int page, int pageSize);
        PagedResult<ArticleInfo> FilterByRolesAndParent(IList<string>? roles, long? parentId, int page, int pageSize);
        ArticleInfo GetById(int articleId);
        ArticleInfo Insert(ArticleInfo article);
        ArticleInfo Update(ArticleInfo article);
    }
}
