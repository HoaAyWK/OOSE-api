using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DbSets.Incoming;

namespace OpenRequest.DataService.IRepository;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<bool> UpdateUserProfile(User user);

    Task<User> GetByIdentityId(Guid identityId);
    Task<bool> UpdateAvatar(Guid userId, string filePath);
    Task<bool> UpdateBackground(Guid userId, string filePath);
    Task<bool> UpdateUserInfo(Guid userId, UpdateUserInfoDto updateUserInfoDto);
}
