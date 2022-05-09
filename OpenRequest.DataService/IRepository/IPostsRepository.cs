using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IPostsRepository : IGenericRepository<Post>
{
    Task<IEnumerable<Post>> GetActivePosts();
    Task<IEnumerable<Post>> GetCustomerPosts(Guid customerId);
    Task<IEnumerable<Post>> GetCustomerActivePosts(Guid customerId);
    Task<IEnumerable<Post>> GetHighestPricePosts(int number);
    Task<bool> Close(Guid id);
    Task<bool> Process(Guid id, Guid freelancerId);
    Task<int> GetStatus(Guid id);
    Task<List<Post>> GetPostsByCategory(Guid id);
    Task<List<Post>> SearchPosts(string name);
}
