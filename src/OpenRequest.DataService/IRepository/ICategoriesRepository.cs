using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface ICategoriesRepository : IGenericRepository<Category>
{
    Task<Category> GetCategoryById(Guid id);
}
