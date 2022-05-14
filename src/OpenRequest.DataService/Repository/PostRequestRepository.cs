using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class PostRequestRepository : GenericRepository<PostRequest>, IPostRequestRepository
{
    public PostRequestRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public async Task<bool> Delete(Guid postid, Guid freelancerId)
    {
        try 
        {
            var existingPR = await dbSet.Where(pr => pr.PostId == postid && pr.FreelancerId == freelancerId)
                .FirstOrDefaultAsync();
            if (existingPR != null)
            {
                dbSet.Remove(existingPR);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete method error.", typeof(PostRequestRepository));
            return false;
        }
    }


    public async Task<bool> Select(Guid postId, Guid freelancerId)
    {
        var existingPR = await dbSet.Where(pr => pr.PostId == postId && pr.FreelancerId == freelancerId)
            .FirstOrDefaultAsync();     
        if (existingPR == null)
        {
            return false;
        }

        existingPR.Status = 1;
        return true;
    }

    public async Task<IEnumerable<PostRequest>> GetPostRequestsByPostId(Guid postId)
    {
        var postRequests = await dbSet.Where(pr => pr.PostId == postId)
            .Include(pr => pr.Freelancer)
            .AsNoTracking()
            .ToListAsync();
        
        return postRequests;
    }
}