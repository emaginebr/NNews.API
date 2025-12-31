using NNews.Dtos;

namespace NNews.ACL.Interfaces
{
    public interface ICategoryClient
    {
        Task<IList<CategoryInfo>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IList<CategoryInfo>> FilterAsync(IList<string>? roles = null, long? parentId = null, CancellationToken cancellationToken = default);
        Task<CategoryInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoryInfo> CreateAsync(CategoryInfo category, CancellationToken cancellationToken = default);
        Task<CategoryInfo> UpdateAsync(CategoryInfo category, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
