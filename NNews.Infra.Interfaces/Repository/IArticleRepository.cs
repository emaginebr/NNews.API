namespace NNews.Infra.Interfaces.Repository
{
    public interface IArticleRepository<TModel>
    {
        (IEnumerable<TModel> Items, int TotalCount) ListAll(long? categoryId, int page, int pageSize);
        (IEnumerable<TModel> Items, int TotalCount) ListByRoles(IList<string>? roles, int page, int pageSize);
        (IEnumerable<TModel> Items, int TotalCount) ListByTag(IList<string>? roles, string tagSlug, int page, int pageSize);
        (IEnumerable<TModel> Items, int TotalCount) ListByCategory(IList<string>? roles, long categoryId, int page, int pageSize);
        (IEnumerable<TModel> Items, int TotalCount) Search(IList<string>? roles, string keyword, int page, int pageSize);
        TModel GetById(int id);
        IEnumerable<TModel> GetScheduledArticles();
        int CountByCategoryId(int categoryId);
        TModel Insert(TModel category);
        TModel Update(TModel category);
        void Delete(int id);
    }
}
