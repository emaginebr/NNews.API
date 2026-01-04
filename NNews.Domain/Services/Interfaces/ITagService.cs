using NNews.DTO;

namespace NNews.Domain.Services.Interfaces
{
    public interface ITagService
    {
        IList<TagInfo> ListAll();
        IList<TagInfo> ListByRoles(IList<string>? roles);
        TagInfo GetById(int tagId);
        Task<TagInfo> InsertAsync(TagInfo tag);
        Task<TagInfo> UpdateAsync(TagInfo tag);
        void Delete(int tagId);
        void MergeTags(long sourceTagId, long targetTagId);
    }
}
