namespace NNews.Infra.Interfaces.Repository
{
    public interface ICategoryRepository<TModel>
    {
        IEnumerable<TModel> ListAll();
        IEnumerable<TModel> ListByParent(IList<string>? roles, long? parentId);
        TModel GetById(int id);
        TModel? GetByTitle(string title);
        TModel Insert(TModel category);
        TModel Update(TModel category);
        void Delete(int id);
    }
}
