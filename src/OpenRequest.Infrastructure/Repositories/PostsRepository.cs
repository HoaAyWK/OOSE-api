using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Core.Entities;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class PostsRepository : GenericRepository<Post>, IPostsRepository
{
    public PostsRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public async Task<IEnumerable<Post>> GetPosts(int? status = 1)
    {
        return await dbSet.Where(p => p.Status == status)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPostsByAuthor(Guid id, int status = 1)
    {
        return await dbSet.Where(p => p.Status == status && p.AuthorId == id)
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .OrderByDescending(p => p.CreatedDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Post> GetById(Guid id)
    {
        return await dbSet.Where(p => p.Id == id)
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.FreelancerRequests)
                .ThenInclude(fr => fr.Freelancer)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<Post> GetProcessingOrClosedPostAsync(Guid id)
    {
        return await dbSet.Where(p => p.Id == id)
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.FreelancerRequests)
            .Include(p => p.Freelancer)
            .Include(p => p.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Post>> GetProcessingPostAlmostEnd()
    {
        var timespan = new TimeSpan(10, 0, 0);
        return await dbSet.Include(p => p.Freelancer)
            .Include(p => p.Assignment)
            .Where(p => p.Status == 2 && 
            p.Assignment.EndDate.Subtract(DateTime.Now) < timespan)
            .ToListAsync();
    }

    public async Task<bool> Update(Guid id, Post post)
    {
        try
        {
            var existingPost = await dbSet.Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (existingPost == null)
            {
                return false;
            }

            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.Status = post.Status;
            existingPost.UpdatedDate = DateTime.UtcNow;
            existingPost.FeaturedImage = post.FeaturedImage;
            existingPost.Price = post.Price;
            existingPost.Duration = post.Duration;                
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Upsert method error.", typeof(PostsRepository));
            return false;
        }
    }

    public override async Task<bool> Delete(Guid id)
    {
        try
        {
            var existingPost = await dbSet.Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (existingPost == null)
            {
                return false;
            }

            dbSet.Remove(existingPost);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Repo} Delete method error.", typeof(PostsRepository));
            return false;
        }
    }

    public async Task<bool> Process(Guid id, Guid freelancerId)
    {
        var existingPost = await dbSet.Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (existingPost == null)
        {
            return false;
        }

        existingPost.Status = 2;
        existingPost.FreelancerId = freelancerId;
        return true;
    }

    public async Task<bool> Submit(Guid id)
    {
        var existingPost = await dbSet.Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        if (existingPost == null)
        {
            return false;
        }

        existingPost.Status = 0;
        return true;
    }
}