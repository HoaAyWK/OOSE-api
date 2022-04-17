using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<bool> UpdateUserProfile(User user);

    Task<User> GetByIdentityId(Guid identityId);
}
