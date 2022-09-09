using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IPostCategoryRepository : IGenericRepository<PostCategory>
{
    bool ExistingAnyPost(Guid categoryId);
    Task<IEnumerable<PostCategory>> GetByPostId(Guid postId);
    Task<bool> Delete(PostCategory postCategory);
}
