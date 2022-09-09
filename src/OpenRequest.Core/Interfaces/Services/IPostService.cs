using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Dtos.Posts;

namespace OpenRequest.Core.Interfaces.Services;

public interface IPostService
{
    Task<Result<IEnumerable<PostResponse>>> GetPostsAsync(int status = 1);
    Task<Result<IEnumerable<PostResponse>>> GetPostsByAuthorAsync(Guid id, int status = 1);
    Task<Result<IEnumerable<PostResponse>>> GetOwnerPostsAsync(string token, int status = 1);
    Task<Result<PostDetailResponse>> GetPostByIdAsync(Guid id);
    Task<Result<PostFullDetailResponse>> GetProcessingOrClosedPostAsync(string token, Guid id);
    Task<Result<PostResponse>> CreateAsync(string token, PostRequest request);
    Task<Result<string>> UpdateAsync(string token, Guid id, PostRequest request);
    Task<Result<string>> DeleteAsync(string token, Guid id);
    Task<Result<string>> SelectAsync(string token, Guid id);
    Task<Result<string>> UnselectAsync(string token, Guid id);
    Task<Result<string>> SelectFreelancerAsync(string token, Guid postId, Guid freelancerId);
    Task<Result<IEnumerable<PostFullDetailResponse>>> GetPostsAlmostEnd();
}