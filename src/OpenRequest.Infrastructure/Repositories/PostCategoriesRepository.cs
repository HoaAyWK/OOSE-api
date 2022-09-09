using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class PostCategoryRepository : GenericRepository<PostCategory>, IPostCategoryRepository
{
    public PostCategoryRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public bool ExistingAnyPost(Guid categoryId)
    {
        return dbSet.Where(x => x.CategoryId == categoryId).Any();
    }

    public async Task<IEnumerable<PostCategory>> GetByPostId(Guid postId)
    {
        return await dbSet.Where(pc => pc.PostId == postId)
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<bool> Delete(PostCategory postCategory)
    {
        try 
        {
            var existingPC =  await dbSet.Where(pc => 
                pc.PostId == postCategory.PostId && 
                pc.CategoryId == postCategory.CategoryId)
                .FirstOrDefaultAsync();
            
            if (existingPC != null)
            {
                dbSet.Remove(existingPC);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Repo} Delete method error.", typeof(PostCategoryRepository));
            return false;
        }
    }
}