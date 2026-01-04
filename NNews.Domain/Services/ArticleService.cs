using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;
using NNews.Infra.Interfaces.Repository;
using NTools.ACL.Interfaces;

namespace NNews.Domain.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository<IArticleModel> _articleRepository;
        private readonly ITagRepository<ITagModel> _tagRepository;
        private readonly IMapper _mapper;
        private readonly IStringClient _stringClient;

        public ArticleService(
            IArticleRepository<IArticleModel> articleRepository, 
            ITagRepository<ITagModel> tagRepository,
            IMapper _mapper,
            IStringClient stringClient)
        {
            _articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            this._mapper = _mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _stringClient = stringClient ?? throw new ArgumentNullException(nameof(stringClient));
        }

        public PagedResult<ArticleInfo> ListAll(long? categoryId, int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.ListAll(categoryId, page, pageSize);
            var articles = _mapper.Map<IList<ArticleInfo>>(items);

            return new PagedResult<ArticleInfo>
            {
                Items = articles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public PagedResult<ArticleInfo> ListByRoles(IList<string>? roles, int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.ListByRoles(roles, page, pageSize);
            var articles = _mapper.Map<IList<ArticleInfo>>(items);

            return new PagedResult<ArticleInfo>
            {
                Items = articles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public PagedResult<ArticleInfo> ListByTag(IList<string>? roles, string tagSlug, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(tagSlug))
                throw new ArgumentException("Tag slug cannot be empty.", nameof(tagSlug));

            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.ListByTag(roles, tagSlug, page, pageSize);
            var articles = _mapper.Map<IList<ArticleInfo>>(items);

            return new PagedResult<ArticleInfo>
            {
                Items = articles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public PagedResult<ArticleInfo> ListByCategory(IList<string>? roles, long categoryId, int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.ListByCategory(roles, categoryId, page, pageSize);
            var articles = _mapper.Map<IList<ArticleInfo>>(items);

            return new PagedResult<ArticleInfo>
            {
                Items = articles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public PagedResult<ArticleInfo> Search(IList<string>? roles, string keyword, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Search keyword cannot be empty.", nameof(keyword));

            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.Search(roles, keyword, page, pageSize);
            var articles = _mapper.Map<IList<ArticleInfo>>(items);

            return new PagedResult<ArticleInfo>
            {
                Items = articles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public ArticleInfo GetById(int articleId)
        {
            var article = _articleRepository.GetById(articleId);
            return _mapper.Map<ArticleInfo>(article);
        }

        public ArticleInfo Insert(ArticleInsertedInfo article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            if (string.IsNullOrWhiteSpace(article.Title))
                throw new ArgumentException("Article title cannot be empty.", nameof(article));

            if (string.IsNullOrWhiteSpace(article.Content))
                throw new ArgumentException("Article content cannot be empty.", nameof(article));

            if (article.CategoryId <= 0)
                throw new ArgumentException("Article must have a valid category.", nameof(article));

            if (article.DateAt == default)
                article.DateAt = DateTime.UtcNow;

            var articleModel = _mapper.Map<ArticleModel>(article);

            ProcessTagsAsync(articleModel, article.TagList).GetAwaiter().GetResult();

            ProcessRoles(articleModel, article.Roles);

            var insertedArticle = _articleRepository.Insert(articleModel);
            return _mapper.Map<ArticleInfo>(insertedArticle);
        }

        public ArticleInfo Update(ArticleUpdatedInfo article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            if (string.IsNullOrWhiteSpace(article.Title))
                throw new ArgumentException("Article title cannot be empty.", nameof(article.Title));

            if (string.IsNullOrWhiteSpace(article.Content))
                throw new ArgumentException("Article content cannot be empty.", nameof(article.Content));

            if (article.CategoryId <= 0)
                throw new ArgumentException("Article must have a valid category.", nameof(article.CategoryId));

            if (article.DateAt == default)
                throw new ArgumentException("Article date cannot be empty.", nameof(article.DateAt));

            var articleModel = _mapper.Map<ArticleModel>(article);

            ProcessTagsAsync(articleModel, article.TagList).GetAwaiter().GetResult();

            ProcessRoles(articleModel, article.Roles);

            var updatedArticle = _articleRepository.Update(articleModel);
            return _mapper.Map<ArticleInfo>(updatedArticle);
        }

        public ArticleInfo Schedule(int articleId, DateTime publishDate)
        {
            if (publishDate <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled date must be in the future.", nameof(publishDate));

            var articleModel = _articleRepository.GetById(articleId);
            
            if (articleModel == null)
                throw new KeyNotFoundException($"Article with ID {articleId} not found.");

            var mutableArticle = articleModel as ArticleModel;
            if (mutableArticle == null)
                throw new InvalidOperationException("Unable to modify article.");

            mutableArticle.Schedule(publishDate);
            var updatedArticle = _articleRepository.Update(mutableArticle);
            return _mapper.Map<ArticleInfo>(updatedArticle);
        }

        public void PublishScheduledArticles()
        {
            var scheduledArticles = _articleRepository.GetScheduledArticles();

            foreach (var article in scheduledArticles)
            {
                var mutableArticle = article as ArticleModel;
                if (mutableArticle != null)
                {
                    mutableArticle.PublishIfScheduled();
                    if (mutableArticle.IsPublished())
                    {
                        _articleRepository.Update(mutableArticle);
                    }
                }
            }
        }

        private async Task ProcessTagsAsync(ArticleModel articleModel, string? tagList)
        {
            if (string.IsNullOrWhiteSpace(tagList))
                return;

            var tagNames = tagList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var tagName in tagNames)
            {
                var slug = await _stringClient.GenerateSlugAsync(tagName);
                var existingTag = _tagRepository.GetBySlug(slug);

                if (existingTag != null)
                {
                    articleModel.AddTag(existingTag as TagModel ?? TagModel.Reconstruct(existingTag.TagId, existingTag.Title, existingTag.Slug));
                }
                else
                {
                    var newTag = _tagRepository.Insert(TagModel.Create(tagName, slug));
                    articleModel.AddTag(newTag as TagModel ?? TagModel.Reconstruct(newTag.TagId, newTag.Title, newTag.Slug));
                }
            }
        }

        private void ProcessRoles(ArticleModel articleModel, List<string>? roles)
        {
            if (roles == null || !roles.Any())
                return;

            foreach (var roleSlug in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var roleName = roleSlug;
                articleModel.AddRole(RoleModel.Create(roleSlug, roleName));
            }
        }
    }
}
