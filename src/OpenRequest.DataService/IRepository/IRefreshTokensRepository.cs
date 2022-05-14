
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IRefreshTokensRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken> GetByRefreshToken(string refreshToken);
    Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
}