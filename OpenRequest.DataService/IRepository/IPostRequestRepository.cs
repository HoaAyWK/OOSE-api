using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IPostRequestRepository : IGenericRepository<PostRequest>
{
    Task<bool> Select(Guid postId, Guid freelancerId);
    Task<bool> Delete(Guid postId, Guid freelancerId);
    Task<IEnumerable<PostRequest>> GetPostRequestsByPostId(Guid postId);
}
