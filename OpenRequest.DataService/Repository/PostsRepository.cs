using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class PostsRepository : GenericRepository<Post>, IPostsRepository
{
    public PostsRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }
}