using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IRefreshTokensRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByRefreshToken(string refreshToken);

    Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
}