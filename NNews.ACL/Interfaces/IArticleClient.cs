using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNews.DTO;

namespace NNews.ACL.Interfaces
{
    public interface IArticleClient
    {
        Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<ArticleInfo>> ListByCategoryAsync(long categoryId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<ArticleInfo>> ListByRolesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<ArticleInfo>> ListByTagAsync(string tagSlug, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<ArticleInfo>> SearchAsync(string keyword, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<ArticleInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ArticleInfo> CreateAsync(ArticleInsertedInfo article, CancellationToken cancellationToken = default);
        Task<ArticleInfo> UpdateAsync(ArticleUpdatedInfo article, CancellationToken cancellationToken = default);
    }
}
