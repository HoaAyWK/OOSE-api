using OpenRequest.Core.Dtos.Users;

namespace OpenRequest.Core.Dtos.Posts;

public class PostDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public double Duration { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public AuthorResponse? Author { get; set; }
    public ICollection<PostCategoryResponse>? PostCategories { get; set; }
    public ICollection<FreelancerRequestResponse>? FreelancerRequests { get; set; }
}