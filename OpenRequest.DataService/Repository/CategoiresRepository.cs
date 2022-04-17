using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class CategoriesRepository : GenericRepository<Category>, ICategoriesRepository
{
    public CategoriesRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }
}