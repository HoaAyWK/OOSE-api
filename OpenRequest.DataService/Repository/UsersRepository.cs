using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DbSets.Incoming;

namespace OpenRequest.DataService.Repository;

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
    public async Task<User> GetByIdentityId(Guid identityId)
    {
        try
        {
            return await dbSet.Where(x => x.IdentityId == identityId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByIdentityId method has generated an error", typeof(UsersRepository));
            return null;
        }
    }

    public Task<bool> UpdateUserProfile(User user)
    {
        throw new NotImplementedException();
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

    public async Task<bool> UpdateUserInfo(Guid userId, UpdateUserInfoDto updateUserInfoDto)
    {
        var user = await dbSet.Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return false;
        }
        
        user.FirstName = updateUserInfoDto.FirstName;
        user.LastName = updateUserInfoDto.LastName;
        user.Phone = updateUserInfoDto.LastName;
        user.DateOfBirth = Convert.ToDateTime(updateUserInfoDto.DateOfBirth);
        user.Address = updateUserInfoDto.Address;
        user.Country = updateUserInfoDto.Country;
        return true;
    }
}