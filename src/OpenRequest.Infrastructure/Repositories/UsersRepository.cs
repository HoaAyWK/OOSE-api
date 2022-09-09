using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Core.Entities;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public override async Task<IEnumerable<User>> All()
    {
        return await dbSet.OrderBy(x => x.FirstName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdentityId(Guid identityId)
    {
        return await dbSet
            .Where(x => x.IdentityId == identityId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAvatar(Guid id, string filePath)
    {
        var user = await dbSet.Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (user == null) 
        {
            return false;
        }

        user.FeaturedAvatar = filePath;

        return true;
    }

    public async Task<bool> UpdateBackground(Guid id, string filePath)
    {
        var user = await dbSet.Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (user == null) 
        {
            return false;
        }
        user.FeaturedBackground = filePath;

        return true;
    }

    public async Task<bool> UpdateUserInfo(Guid userId, User userToUpdate)
    {
        var user = await dbSet.Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return false;
        }
        
        user.FirstName = userToUpdate.FirstName;
        user.LastName = userToUpdate.LastName;
        user.Phone = userToUpdate.LastName;
        user.Address = userToUpdate.Address;
        user.Country = userToUpdate.Country;

        return true;
    }
}