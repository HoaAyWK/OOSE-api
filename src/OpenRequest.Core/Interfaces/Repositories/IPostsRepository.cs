using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IPostsRepository : IGenericRepository<Post>
{
    Task<IEnumerable<Post>> GetPosts(int? status = 1);
    Task<IEnumerable<Post>> GetPostsByAuthor(Guid id, int status = 1);
    Task<Post> GetProcessingOrClosedPostAsync(Guid id);
    Task<IEnumerable<Post>> GetProcessingPostAlmostEnd();

    Task<Post> Create(Post post);
    Task<bool> Update(Guid id, Post post);
    Task<bool> Process(Guid id, Guid freelancerId);
    Task<bool> Submit(Guid id);
}
