using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;

namespace NNews.Domain.Entities
{
    public class ArticleModel : IArticleModel
    {
        private readonly List<ITagModel> _tags;
        private readonly List<IRoleModel> _roles;

        public long ArticleId { get; private set; }
        public long CategoryId { get; private set; }
        public long? AuthorId { get; private set; }
        public DateTime DateAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public ArticleStatus Status { get; private set; }
        public ICategoryModel? Category { get; private set; }

        public IReadOnlyCollection<ITagModel> Tags => _tags.AsReadOnly();
        public IReadOnlyCollection<IRoleModel> Roles => _roles.AsReadOnly();

        private ArticleModel()
        {
            _tags = new List<ITagModel>();
            _roles = new List<IRoleModel>();
            Title = string.Empty;
            Content = string.Empty;
        }

        public ArticleModel(string title, string content, long categoryId, long? authorId = null, ArticleStatus status = ArticleStatus.Draft) : this()
        {
            SetTitle(title);
            SetContent(content);
            SetCategoryId(categoryId);
            if (authorId.HasValue)
                SetAuthorId(authorId.Value);
            Status = status;
            DateAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static ArticleModel Create(string title, string content, long categoryId, long? authorId = null, ArticleStatus status = ArticleStatus.Draft)
        {
            return new ArticleModel(title, content, categoryId, authorId, status);
        }

        public static ArticleModel Reconstruct(long articleId, string title, string content,
            long categoryId, long? authorId, ArticleStatus status, DateTime dateAt, DateTime createdAt, DateTime updatedAt)
        {
            var article = new ArticleModel
            {
                ArticleId = articleId,
                Title = title,
                Content = content,
                CategoryId = categoryId,
                AuthorId = authorId,
                Status = status,
                DateAt = dateAt,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return article;
        }

        public void SetCategory(ICategoryModel? category)
        {
            Category = category;
        }

        public void UpdateTitle(string title)
        {
            SetTitle(title);
            UpdateTimestamp();
        }

        public void UpdateContent(string content)
        {
            SetContent(content);
            UpdateTimestamp();
        }

        public void Update(string title, string content)
        {
            SetTitle(title);
            SetContent(content);
            UpdateTimestamp();
        }

        public void ChangeCategory(long categoryId)
        {
            SetCategoryId(categoryId);
            UpdateTimestamp();
        }

        public void Publish()
        {
            if (Status == ArticleStatus.Published)
                return;

            ValidateForPublishing();
            Status = ArticleStatus.Published;
            UpdateTimestamp();
        }

        public void Draft()
        {
            if (Status == ArticleStatus.Draft)
                return;

            Status = ArticleStatus.Draft;
            UpdateTimestamp();
        }

        public void Archive()
        {
            if (Status == ArticleStatus.Archived)
                return;

            Status = ArticleStatus.Archived;
            UpdateTimestamp();
        }

        public void Schedule(DateTime publishDate)
        {
            if (publishDate <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled date must be in the future.", nameof(publishDate));

            ValidateForPublishing();
            Status = ArticleStatus.Scheduled;
            DateAt = publishDate;
            UpdateTimestamp();
        }

        public void PublishIfScheduled()
        {
            if (Status != ArticleStatus.Scheduled)
                return;

            if (DateAt <= DateTime.UtcNow)
            {
                Status = ArticleStatus.Published;
                UpdateTimestamp();
            }
        }

        public void AddTag(ITagModel tag)
        {
            //if (tag.TagId <= 0)
            //    throw new ArgumentException("Tag ID must be greater than zero.", nameof(tag));

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                UpdateTimestamp();
            }
        }

        public void RemoveTag(long tagId)
        {
            _tags.RemoveAll(t => t.TagId == tagId);
            UpdateTimestamp();
        }

        public void AddRole(IRoleModel role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (string.IsNullOrWhiteSpace(role.Slug))
            {
                throw new ArgumentException("Role slug is empty.", nameof(role));
            }

            if (!_roles.Any(r => r.Slug.Equals(role.Slug, StringComparison.OrdinalIgnoreCase)))
            {
                _roles.Add(role);
                UpdateTimestamp();
            }
        }

        public void RemoveRole(string slug)
        {
            _roles.RemoveAll(r => r.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            UpdateTimestamp();
        }

        public bool IsPublished() => Status == ArticleStatus.Published;

        public bool IsDraft() => Status == ArticleStatus.Draft;

        public bool IsArchived() => Status == ArticleStatus.Archived;

        public bool IsScheduled() => Status == ArticleStatus.Scheduled;

        public int GetTagCount() => _tags.Count;

        public int GetRoleCount() => _roles.Count;

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (title.Length > 255)
                throw new ArgumentException("Title cannot exceed 255 characters.", nameof(title));

            Title = title.Trim();
        }

        private void SetContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));

            Content = content.Trim();
        }

        private void SetCategoryId(long categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than zero.", nameof(categoryId));

            CategoryId = categoryId;
        }

        private void SetAuthorId(long authorId)
        {
            if (authorId <= 0)
                throw new ArgumentException("Author ID must be greater than zero.", nameof(authorId));

            AuthorId = authorId;
        }

        private void ValidateForPublishing()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new InvalidOperationException("Cannot publish article without a title.");

            if (string.IsNullOrWhiteSpace(Content))
                throw new InvalidOperationException("Cannot publish article without content.");

            if (CategoryId <= 0)
                throw new InvalidOperationException("Cannot publish article without a category.");
        }

        private void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ArticleModel other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (ArticleId == 0 || other.ArticleId == 0)
                return false;

            return ArticleId == other.ArticleId;
        }

        public override int GetHashCode()
        {
            return ArticleId.GetHashCode();
        }

        public override string ToString()
        {
            var scheduledInfo = Status == ArticleStatus.Scheduled ? $" (Scheduled: {DateAt:u})" : string.Empty;
            return $"Article: {Title} (Status: {Status}{scheduledInfo}) - {GetTagCount()} tags, {GetRoleCount()} roles";
        }
    }
}
