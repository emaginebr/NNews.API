namespace NNews.Infra.Interfaces.Repository
{
    public interface IArticleRepository<TModel>
    {
        (IEnumerable<TModel> Items, int TotalCount) ListAll(long? categoryId, int page, int pageSize);
        (IEnumerable<TModel> Items, int TotalCount) FilterByRolesAndParent(IList<string>? roles, long? parentId, int page, int pageSize);
        TModel GetById(int id);
        IEnumerable<TModel> GetScheduledArticles();
        int CountByCategoryId(int categoryId);
        TModel Insert(TModel category);
        TModel Update(TModel category);
        void Delete(int id);
    }
}
