using NNews.Domain.Enums;

namespace NNews.Domain.Entities.Interfaces
{
    public interface IArticleModel
    {
        long ArticleId { get; }
        long CategoryId { get; }
        long? AuthorId { get; }
        DateTime DateAt { get; }
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
        string Title { get; }
        string Content { get; }
        ArticleStatus Status { get; }
        ICategoryModel? Category { get; }
        IReadOnlyCollection<ITagModel> Tags { get; }
        IReadOnlyCollection<IRoleModel> Roles { get; }

        void UpdateTitle(string title);
        void UpdateContent(string content);
        void Update(string title, string content);
        void ChangeCategory(long categoryId);
        void Publish();
        void Draft();
        void Archive();
        void Schedule(DateTime publishDate);
        void PublishIfScheduled();
        void AddTag(ITagModel tag);
        void RemoveTag(long tagId);
        void AddRole(IRoleModel role);
        void RemoveRole(string slug);
        bool IsPublished();
        bool IsDraft();
        bool IsArchived();
        bool IsScheduled();
        int GetTagCount();
        int GetRoleCount();
    }
}
