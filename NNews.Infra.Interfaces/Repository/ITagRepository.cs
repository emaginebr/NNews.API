namespace NNews.Infra.Interfaces.Repository
{
    public interface ITagRepository<TModel>
    {
        IEnumerable<TModel> ListAll();
        IEnumerable<TModel> ListByRoles(IList<string>? roles);
        TModel GetById(int id);
        TModel? GetBySlug(string slug);
        bool ExistSlug(string slug);
        bool ExistsByTitle(string title, long? excludeTagId = null);
        TModel Insert(TModel category);
        TModel Update(TModel category);
        void Delete(int id);
        void MergeTags(long sourceTagId, long targetTagId);
    }
}
