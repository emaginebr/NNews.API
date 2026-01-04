using NNews.DTO;

namespace NNews.Domain.Services.Interfaces
{
    public interface ICategoryService
    {
        IList<CategoryInfo> ListAll();
        IList<CategoryInfo> ListByParent(IList<string>? roles, long? parentId);
        CategoryInfo GetById(int categoryId);
        CategoryInfo Insert(CategoryInfo category);
        CategoryInfo Update(CategoryInfo category);
        void Delete(int categoryId);
    }
}
