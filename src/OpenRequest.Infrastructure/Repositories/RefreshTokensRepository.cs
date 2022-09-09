using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class RefreshTokensRepository : GenericRepository<RefreshToken>, IRefreshTokensRepository
{
    public RefreshTokensRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<RefreshToken?> GetByRefreshToken(string refreshToken)
    {
        try
        {
            return await dbSet
                .Where(x => x.Token.ToLower() == refreshToken.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokensRepository));
            return null;
        }
    }

    public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
    {
        try
        {
            var token = await dbSet
                .Where(x => x.Token.ToLower() == refreshToken.Token.ToLower())
                .FirstOrDefaultAsync();
            
            if (token == null) 
                return false;

            token.IsUsed = refreshToken.IsUsed;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokensRepository));
            
            return false;
        }
    }
}