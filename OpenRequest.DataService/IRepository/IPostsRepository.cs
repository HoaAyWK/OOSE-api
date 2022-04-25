using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IPostsRepository : IGenericRepository<Post>
{
    Task<IEnumerable<Post>> GetCustomerPosts(Guid customerId);
    Task<bool> Close(Guid id);
    Task<bool> Process(Guid id);
    Task<int> GetStatus(Guid id);
    Task<List<Post>> GetPostsByCategory(Guid id);
    Task<List<Post>> SearchPosts(string name);
}
