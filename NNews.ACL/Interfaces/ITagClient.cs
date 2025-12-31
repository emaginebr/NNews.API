using NNews.Dtos;

namespace NNews.ACL.Interfaces
{
    public interface ITagClient
    {
        Task<IList<TagInfo>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TagInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TagInfo> CreateAsync(TagInfo tag, CancellationToken cancellationToken = default);
        Task<TagInfo> UpdateAsync(TagInfo tag, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task MergeTagsAsync(long sourceTagId, long targetTagId, CancellationToken cancellationToken = default);
    }
}
