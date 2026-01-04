using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;
using NNews.Infra.Interfaces.Repository;

namespace NNews.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository<ICategoryModel> _categoryRepository;
        private readonly IArticleRepository<IArticleModel> _articleRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository<ICategoryModel> categoryRepository, IArticleRepository<IArticleModel> articleRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<CategoryInfo> ListAll()
        {
            var categories = _categoryRepository.ListAll();
            return _mapper.Map<IList<CategoryInfo>>(categories);
        }

        public IList<CategoryInfo> ListByParent(IList<string>? roles, long? parentId)
        {
            var categories = _categoryRepository.ListByParent(roles, parentId);
            return _mapper.Map<IList<CategoryInfo>>(categories);
        }

        public CategoryInfo GetById(int categoryId)
        {
            var category = _categoryRepository.GetById(categoryId);
            return _mapper.Map<CategoryInfo>(category);
        }

        public CategoryInfo Insert(CategoryInfo category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Title))
                throw new ArgumentException("Category title cannot be empty.", nameof(category.Title));

            if (category.ParentId.HasValue && category.ParentId.Value <= 0)
                throw new ArgumentException("Parent ID must be greater than zero if provided.", nameof(category.ParentId));

            if (category.ParentId.HasValue)
            {
                try
                {
                    _categoryRepository.GetById((int)category.ParentId.Value);
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Parent category with ID {category.ParentId.Value} not found.", nameof(category.ParentId));
                }
            }

            var existingCategory = _categoryRepository.GetByTitle(category.Title);
            if (existingCategory != null)
                throw new InvalidOperationException($"A category with the title '{category.Title}' already exists.");

            var categoryModel = _mapper.Map<CategoryModel>(category);
            var insertedCategory = _categoryRepository.Insert(categoryModel);
            return _mapper.Map<CategoryInfo>(insertedCategory);
        }

        public CategoryInfo Update(CategoryInfo category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Title))
                throw new ArgumentException("Category title cannot be empty.", nameof(category.Title));

            if (category.ParentId.HasValue && category.ParentId.Value <= 0)
                throw new ArgumentException("Parent ID must be greater than zero if provided.", nameof(category.ParentId));

            if (category.ParentId.HasValue)
            {
                if (category.ParentId.Value == category.CategoryId)
                    throw new ArgumentException("A category cannot be its own parent.", nameof(category.ParentId));

                try
                {
                    _categoryRepository.GetById((int)category.ParentId.Value);
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Parent category with ID {category.ParentId.Value} not found.", nameof(category.ParentId));
                }
            }

            var existingCategory = _categoryRepository.GetByTitle(category.Title);
            if (existingCategory != null && existingCategory.CategoryId != category.CategoryId)
                throw new InvalidOperationException($"A category with the title '{category.Title}' already exists.");

            var categoryModel = _mapper.Map<CategoryModel>(category);
            var updatedCategory = _categoryRepository.Update(categoryModel);
            return _mapper.Map<CategoryInfo>(updatedCategory);
        }

        public void Delete(int categoryId)
        {
            var articleCount = _articleRepository.CountByCategoryId(categoryId);
            if (articleCount > 0)
                throw new InvalidOperationException($"Cannot delete category because it has {articleCount} article(s) associated with it.");

            var subCategories = _categoryRepository.ListAll()
                .Where(c => c.ParentId == categoryId)
                .ToList();

            if (subCategories.Any())
                throw new InvalidOperationException($"Cannot delete category because it has {subCategories.Count} subcategory(ies) associated with it.");

            _categoryRepository.Delete(categoryId);
        }
    }
}
