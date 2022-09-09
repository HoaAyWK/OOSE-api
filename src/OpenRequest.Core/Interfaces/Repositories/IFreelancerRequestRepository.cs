using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IFreelancerRequestsRepository : IGenericRepository<FreelancerRequest>
{
    Task<bool> Select(Guid postId, Guid freelancerId);
    Task<bool> Delete(Guid postId, Guid freelancerId);
    Task<IEnumerable<FreelancerRequest>> GetFreelancerRequestssByPostId(Guid postId);
}
