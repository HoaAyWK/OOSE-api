using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public Task<User> GetByIdentityId(Guid identityId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserProfile(User user)
    {
        throw new NotImplementedException();
    }
}