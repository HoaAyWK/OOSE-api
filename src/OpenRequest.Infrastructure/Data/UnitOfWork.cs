using Microsoft.Extensions.Logging;
using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Repositories;
namespace OpenRequest.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public IUsersRepository Users { get; private set; }

    public ICategoriesRepository Categories { get; private set; }

    public IPostsRepository Posts { get; private set; }

    public IPostCategoryRepository PostCategory { get; private set; }

    public IFreelancerRequestsRepository FreelancerRequests { get; private set; }

    public IAssignmentsRepository Assignments { get; private set; }

    public IRefreshTokensRepository RefreshTokens { get; private set; }

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("db_logs");

        Users = new UsersRepository(_context, _logger);
        Categories = new CategoriesRepository(_context, _logger);
        Posts = new PostsRepository(_context, _logger);
        PostCategory = new PostCategoryRepository(_context, _logger);
        FreelancerRequests = new FreelancerRequestsRepository(_context, _logger);
        Assignments = new AssignmentsRepository(_context, _logger);
        RefreshTokens = new RefreshTokensRepository(_context, _logger);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}