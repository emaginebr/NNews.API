using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;
using NNews.Infra.Context;
using NNews.Infra.Interfaces.Repository;

namespace NNews.Infra.Repository
{
    public class ArticleRepository : IArticleRepository<IArticleModel>
    {
        private readonly NNewsContext _context;
        private readonly IMapper _mapper;

        public ArticleRepository(NNewsContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public (IEnumerable<IArticleModel> Items, int TotalCount) ListAll(long? categoryId, int page, int pageSize)
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Include(a => a.ArticleRoles)
                .Include(a => a.Tags)
                .Include(a => a.Category);

            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            var totalCount = query.Count();

            var articles = query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (_mapper.Map<IEnumerable<ArticleModel>>(articles), totalCount);
        }

        public (IEnumerable<IArticleModel> Items, int TotalCount) FilterByRolesAndParent(IList<string>? roles, long? parentId, int page, int pageSize)
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Include(a => a.ArticleRoles)
                .Include(a => a.Tags)
                .Include(a => a.Category)
                .Where(a => a.Status == (int)ArticleStatus.Published);

            if (parentId.HasValue)
            {
                query = query.Where(a => a.Category.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(a => a.Category.ParentId == null);
            }

            if (roles != null && roles.Any())
            {
                query = query.Where(a => a.ArticleRoles.Any(ar => roles.Contains(ar.Slug)));
            }

            var totalCount = query.Count();

            var articles = query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (_mapper.Map<IEnumerable<ArticleModel>>(articles), totalCount);
        }

        public IArticleModel GetById(int id)
        {
            var article = _context.Articles
                .AsNoTracking()
                .Include(a => a.ArticleRoles)
                .Include(a => a.Tags)
                .Include(a => a.Category)
                .FirstOrDefault(a => a.ArticleId == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            return _mapper.Map<ArticleModel>(article);
        }

        public int CountByCategoryId(int categoryId)
        {
            return _context.Articles
                .AsNoTracking()
                .Count(a => a.CategoryId == categoryId);
        }

        public IEnumerable<IArticleModel> GetScheduledArticles()
        {
            var articles = _context.Articles
                .AsNoTracking()
                .Include(a => a.ArticleRoles)
                .Include(a => a.Tags)
                .Include(a => a.Category)
                .Where(a => a.Status == (int)ArticleStatus.Scheduled && a.DateAt <= DateTime.UtcNow)
                .ToList();

            return _mapper.Map<IEnumerable<ArticleModel>>(articles);
        }

        public IArticleModel Insert(IArticleModel articleModel)
        {
            if (articleModel == null)
                throw new ArgumentNullException(nameof(articleModel));

            var article = _mapper.Map<Article>(articleModel);
            article.DateAt = DateTime.SpecifyKind(articleModel.DateAt, DateTimeKind.Unspecified);
            article.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            article.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Articles.Add(article);

            if (articleModel.Tags.Any())
            {
                var tagSlugs = articleModel.Tags.Select(t => t.Slug).ToList();
                var existingTags = _context.Tags
                    .Where(t => tagSlugs.Contains(t.Slug))
                    .ToList();

                article.Tags = existingTags;
            }

            if (articleModel.Roles.Any())
            {
                foreach (var role in articleModel.Roles)
                {
                    article.ArticleRoles.Add(new ArticleRole
                    {
                        Article = article,
                        Slug = role.Slug,
                        Name = role.Name
                    });
                }
            }

            _context.SaveChanges();

            return GetById((int)article.ArticleId);
        }

        public IArticleModel Update(IArticleModel articleModel)
        {
            if (articleModel == null)
                throw new ArgumentNullException(nameof(articleModel));

            var existingArticle = _context.Articles
                .Include(a => a.Tags)
                .Include(a => a.ArticleRoles)
                .FirstOrDefault(a => a.ArticleId == articleModel.ArticleId);

            if (existingArticle == null)
                throw new KeyNotFoundException($"Article with ID {articleModel.ArticleId} not found.");

            existingArticle.Title = articleModel.Title;
            existingArticle.Content = articleModel.Content;
            existingArticle.CategoryId = articleModel.CategoryId;
            existingArticle.Status = (int)articleModel.Status;
            existingArticle.DateAt = DateTime.SpecifyKind(articleModel.DateAt, DateTimeKind.Unspecified);
            existingArticle.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            existingArticle.Tags.Clear();
            if (articleModel.Tags.Any())
            {
                var tagSlugs = articleModel.Tags.Select(t => t.Slug).ToList();
                var tags = _context.Tags
                    .Where(t => tagSlugs.Contains(t.Slug))
                    .ToList();

                foreach (var tag in tags)
                {
                    existingArticle.Tags.Add(tag);
                }
            }

            existingArticle.ArticleRoles.Clear();
            if (articleModel.Roles.Any())
            {
                foreach (var role in articleModel.Roles)
                {
                    existingArticle.ArticleRoles.Add(new ArticleRole
                    {
                        ArticleId = existingArticle.ArticleId,
                        Slug = role.Slug,
                        Name = role.Name
                    });
                }
            }

            _context.Articles.Update(existingArticle);
            _context.SaveChanges();

            return GetById((int)existingArticle.ArticleId);
        }

        public void Delete(int id)
        {
            var article = _context.Articles
                .Include(a => a.ArticleRoles)
                .FirstOrDefault(a => a.ArticleId == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            _context.ArticleRoles.RemoveRange(article.ArticleRoles);
            _context.Articles.Remove(article);
            _context.SaveChanges();
        }
    }
}
