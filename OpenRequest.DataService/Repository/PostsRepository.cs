using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRequest.Configuration.Status;
using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class PostsRepository : GenericRepository<Post>, IPostsRepository
{
    public PostsRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public override async Task<IEnumerable<Post>> All()
    {
        var posts = await dbSet.Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .ToListAsync();
        
        return posts;
    }

    public override async Task<Post> GetById(Guid id)
    {
        var post = await dbSet.Include(p => p.Author)
            .Where(p => p.Id == id)
            .Include(p => p.PostRequests)
                .ThenInclude(pr => pr.Freelancer)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Assignment)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return post;
    }

    public async Task<IEnumerable<Post>> GetActivePosts() 
    {
        var posts = await dbSet.Where(p => p.Status == PostStatus.Open || p.Status == PostStatus.Processing)
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedDate)
            .AsNoTracking()
            .ToListAsync();
        return posts;
    }

    public async Task<IEnumerable<Post>> GetCustomerActivePosts(Guid customerId)
    {
        var posts = await dbSet.Where(p => p.Status == PostStatus.Open && p.AuthorId == customerId)
            .OrderByDescending(p => p.CreatedDate)
            .AsNoTracking()
            .ToListAsync();

        return posts;
    } 

    public async Task<IEnumerable<Post>> GetHighestPricePosts(int number)
    {
        var highestPricePosts = await dbSet.Where(p => p.Status == PostStatus.Open)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Author)
            .OrderByDescending(p => p.Price)
            .AsNoTracking()
            .Take(number)
            .ToListAsync();
        
        return highestPricePosts;
    }
    
    public override async Task<bool> Upsert(Guid id, Post post)
    {
        try
            {
                var existingPost = await dbSet.Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (existingPost == null)
                {
                    return await Add(post);
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
            
            if (existingPost != null)
            {
                dbSet.Remove(existingPost);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete method error.", typeof(PostsRepository));
            return false;
        }
    }

    public async Task<IEnumerable<Post>> GetCustomerPosts(Guid customerId)
    {
        return await dbSet.Where(p => p.AuthorId == customerId)
            .Include(p => p.Author)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Close(Guid id)
    {
        var existingPost = await dbSet.Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        if (existingPost == null) return false;
        existingPost.Status = PostStatus.Closed;
        return true;
    }

    public async Task<bool> Process(Guid id, Guid freelancerId)
    {
        var existingPost = await dbSet.Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        if (existingPost == null) return false;
        existingPost.Status = PostStatus.Processing;
        existingPost.FreelancerId = freelancerId;
        return true;
    }

    public async Task<int> GetStatus(Guid id)
    {
        var existingPost = await dbSet.Where(p => p.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return existingPost.Status;
    }

    public async Task<List<Post>> GetPostsByCategory(Guid id)
    {
        var result = await dbSet.Join(
            _context.PostCategory,
            post => post.Id,
            pc => pc.PostId,
            (post, pc) => new
            {
                Post = post,
                CategoryId = pc.CategoryId
            })
            .Where(p => p.CategoryId == id && p.Post.Status == PostStatus.Open)
            .Select(p => p.Post)
            .Distinct()
            .Include(p => p.Author)
            .AsNoTracking()
            .ToListAsync();
        
        return result;
    }

    public async Task<List<Post>> SearchPosts(string name)
    {
        var result = await dbSet.Where(p => p.Title.Contains(name))
            .AsNoTracking()
            .ToListAsync();
        return result;
    }
}