using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;
using NNews.Infra.Context;
using NNews.Infra.Interfaces.Repository;

namespace NNews.Infra.Repository
{
    public class CategoryRepository : ICategoryRepository<ICategoryModel>
    {
        private readonly NNewsContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(NNewsContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public void Delete(int id)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public ICategoryModel GetById(int id)
        {
            var category = _context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return _mapper.Map<CategoryModel>(category);
        }

        public ICategoryModel? GetByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var category = _context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Title.ToLower() == title.ToLower());

            return category != null ? _mapper.Map<CategoryModel>(category) : null;
        }

        public ICategoryModel Insert(ICategoryModel category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var categoryEntity = _mapper.Map<Category>(category);
            categoryEntity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            categoryEntity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Categories.Add(categoryEntity);
            _context.SaveChanges();

            return _mapper.Map<CategoryModel>(categoryEntity);
        }

        public IEnumerable<ICategoryModel> ListAll()
        {
            var categories = _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Title)
                .ToList();

            return _mapper.Map<IEnumerable<CategoryModel>>(categories);
        }

        public IEnumerable<ICategoryModel> ListByParent(IList<string>? roles, long? parentId)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Include(c => c.Articles)
                    .ThenInclude(a => a.ArticleRoles)
                .Where(c => c.Articles.Any(a => a.Status == (int)ArticleStatus.Published));

            if (parentId.HasValue)
            {
                query = query.Where(c => c.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(c => c.ParentId == null);
            }

            if (roles != null && roles.Any())
            {
                query = query.Where(c => c.Articles.Any(a =>
                    a.Status == (int)ArticleStatus.Published &&
                    (a.ArticleRoles.Any(ar => roles.Contains(ar.Slug)) || !a.ArticleRoles.Any())
                ));
            }

            var categories = query
                .OrderBy(c => c.Title)
                .ToList();

            return _mapper.Map<IEnumerable<CategoryModel>>(categories);
        }

        public ICategoryModel Update(ICategoryModel category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = _context.Categories
                .FirstOrDefault(c => c.CategoryId == category.CategoryId);

            if (existingCategory == null)
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");

            existingCategory.Title = category.Title;
            existingCategory.ParentId = category.ParentId;
            existingCategory.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Categories.Update(existingCategory);
            _context.SaveChanges();

            return _mapper.Map<CategoryModel>(existingCategory);
        }
    }
}
