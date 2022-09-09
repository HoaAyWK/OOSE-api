using OpenRequest.Core.Interfaces.Repositories;

namespace OpenRequest.Core.Interfaces.UoW;

public interface IUnitOfWork
{
    IUsersRepository Users { get; }
    ICategoriesRepository Categories { get; }
    IPostsRepository Posts { get; }
    IPostCategoryRepository PostCategory { get; }
    IFreelancerRequestsRepository FreelancerRequests { get; }
    IAssignmentsRepository Assignments { get; }
    IRefreshTokensRepository RefreshTokens { get; }
    Task CompleteAsync();
}