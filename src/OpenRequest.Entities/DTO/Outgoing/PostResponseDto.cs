using OpenRequest.Entities.DbSets;

namespace OpenRequest.Entities.DTO.Outgoing;

public class PostResponseDto 
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? FeaturedImage { get; set; }
    public double Price { get; set; }
    public double Duration { get; set; }
    public Guid AuthorId { get; set; }
    public Customer? Author { get; set; }
    public List<PostCategory>? PostCategories { get; set; }
    public List<PostRequest>? PostRequests { get; set; }
}