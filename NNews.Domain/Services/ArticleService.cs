using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.Dtos;
using NNews.Infra.Interfaces.Repository;

namespace NNews.Domain.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository<IArticleModel> _articleRepository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository<IArticleModel> articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public PagedResult<ArticleInfo> FilterByRolesAndParent(IList<string>? roles, long? parentId, int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var (items, totalCount) = _articleRepository.FilterByRolesAndParent(roles, parentId, page, pageSize);
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

        public ArticleInfo Insert(ArticleInfo article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            if (string.IsNullOrWhiteSpace(article.Title))
                throw new ArgumentException("Article title cannot be empty.", nameof(article.Title));

            if (string.IsNullOrWhiteSpace(article.Content))
                throw new ArgumentException("Article content cannot be empty.", nameof(article.Content));

            if (article.CategoryId <= 0)
                throw new ArgumentException("Article must have a valid category.", nameof(article.CategoryId));

            var articleModel = _mapper.Map<ArticleModel>(article);
            var insertedArticle = _articleRepository.Insert(articleModel);
            return _mapper.Map<ArticleInfo>(insertedArticle);
        }

        public ArticleInfo Update(ArticleInfo article)
        {
            if (article == null)
                throw new ArgumentNullException(nameof(article));

            if (string.IsNullOrWhiteSpace(article.Title))
                throw new ArgumentException("Article title cannot be empty.", nameof(article.Title));

            if (string.IsNullOrWhiteSpace(article.Content))
                throw new ArgumentException("Article content cannot be empty.", nameof(article.Content));

            if (article.CategoryId <= 0)
                throw new ArgumentException("Article must have a valid category.", nameof(article.CategoryId));

            var articleModel = _mapper.Map<ArticleModel>(article);
            var updatedArticle = _articleRepository.Update(articleModel);
            return _mapper.Map<ArticleInfo>(updatedArticle);
        }
    }
}
