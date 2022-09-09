using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class FreelancerRequestsRepository : GenericRepository<FreelancerRequest>, IFreelancerRequestsRepository
{
    public FreelancerRequestsRepository(AppDbContext context, ILogger logger) : base(context, logger)
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
            _logger.LogError(ex, "{Repo} Delete method error.", typeof(FreelancerRequestsRepository));
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

        return true;
    }

    public async Task<IEnumerable<FreelancerRequest>> GetFreelancerRequestssByPostId(Guid postId)
    {
        var FreelancerRequestss = await dbSet.Where(pr => pr.PostId == postId)
            .Include(pr => pr.Freelancer)
            .AsNoTracking()
            .ToListAsync();
        
        return FreelancerRequestss;
    }
}