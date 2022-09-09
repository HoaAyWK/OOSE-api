using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface ICategoriesRepository : IGenericRepository<Category>
{
    Task<Category> GetCategoryById(Guid id);
}
