using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNews.Dtos;

namespace NNews.ACL.Interfaces
{
    public interface IArticleClient
    {
        Task<PagedResult<ArticleInfo>> GetAllAsync(long? categoryId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<ArticleInfo>> FilterAsync(IList<string>? roles = null, long? parentId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<ArticleInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ArticleInfo> CreateAsync(ArticleInfo article, CancellationToken cancellationToken = default);
        Task<ArticleInfo> UpdateAsync(ArticleInfo article, CancellationToken cancellationToken = default);
    }
}
