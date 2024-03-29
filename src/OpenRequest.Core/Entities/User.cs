namespace OpenRequest.Core.Entities;

public class User
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public int Status { get; set; } = 1;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; }
    public Guid IdentityId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; } = null!;
    public string? FeaturedAvatar { get; set; }
    public string? FeaturedBackground { get; set; }
     public string? Address { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public double Rated { get; set; } = 0.0;

    public ICollection<Post>? Posts { get; set; }
    public ICollection<Post>? AssignedPosts { get; set; }
    public ICollection<FreelancerRequest>? FreelancerRequests { get; set; }
}