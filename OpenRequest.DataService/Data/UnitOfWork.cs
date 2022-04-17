using Microsoft.Extensions.Logging;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.DataService.IRepository;
using OpenRequest.DataService.Repository;

namespace OpenRequest.DataService.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;
    public IUsersRepository Users { get; private set; }
    public ICustomersRepository Customers { get; private set; }
    public IFreelancersRepository Freelancers { get; private set; }
    public ICategoriesRepository Categories { get; private set; }
    public IPostsRepository Posts { get; private set; }

    public IRefreshTokensRepository RefreshTokens { get; private set; }

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("db_logs");

        Users = new UsersRepository(_context, _logger);
        Customers = new CustomersRepository(_context, _logger);
        Freelancers = new FreelancersRepository(_context, _logger);
        Categories = new CategoriesRepository(_context, _logger);
        Posts = new PostsRepository(_context, _logger);
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