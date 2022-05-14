using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class PostCategoryRepository : GenericRepository<PostCategory>, IPostCategoryRepository
{
    public PostCategoryRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public async Task<List<PostCategory>> GetPostCategoriesByPostId(Guid postId)
    {
        var result = await dbSet.Where(pc => pc.PostId == postId)
            .AsNoTracking()
            .ToListAsync();
        return result;
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