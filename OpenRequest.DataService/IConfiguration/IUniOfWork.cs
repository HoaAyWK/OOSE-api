using OpenRequest.DataService.IRepository;

namespace OpenRequest.DataService.IConfiguration;

public interface IUnitOfWork
{
    IUsersRepository Users { get; }
    ICustomersRepository Customers { get; }
    IFreelancersRepository Freelancers { get; }
    ICategoriesRepository Categories { get; }
    IPostsRepository Posts { get; }
    IPostCategoryRepository PostCategory { get; }

    IRefreshTokensRepository RefreshTokens { get; }

    Task CompleteAsync();
}