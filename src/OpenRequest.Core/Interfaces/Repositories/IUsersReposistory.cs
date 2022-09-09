using OpenRequest.Core.Dtos.Users;
using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<User?> GetUserByIdentityId(Guid identityId);
    Task<bool> UpdateAvatar(Guid userId, string filePath);
    Task<bool> UpdateBackground(Guid userId, string filePath);
    Task<bool> UpdateUserInfo(Guid userId, User userToUpdate);
}
