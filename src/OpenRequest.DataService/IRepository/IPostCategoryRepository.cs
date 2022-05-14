using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IPostCategoryRepository : IGenericRepository<PostCategory>
{
    Task<List<PostCategory>> GetPostCategoriesByPostId(Guid id);
    Task<bool> Delete(PostCategory postCategory);
}
