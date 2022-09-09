using OpenRequest.Core.Dtos.Assignments;
using OpenRequest.Core.Dtos.Users;

namespace OpenRequest.Core.Dtos.Posts;

public class PostFullDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public double Duration { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public AuthorResponse? Author { get; set; }
    public FreelancerResponse? Freelancer { get; set; }
    public AssignmentResponse? Assignment { get; set; }
    public ICollection<PostCategoryResponse>? PostCategories { get; set; }
}