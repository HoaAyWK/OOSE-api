namespace OpenRequest.Core.Entities;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Status { get; set; } = 1;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? FeaturedImage { get; set; }

    public double Price { get; set; } 

    public double Duration { get; set; }


    public Guid AuthorId { get; set; }

    public User? Author { get; set; } 

    public Assignment? Assignment { get; set; }


    public List<PostCategory>? PostCategories { get; set; }


    // List of Freelancers that wanna to do the job
    public List<FreelancerRequest>? FreelancerRequests { get; set; }

    // Freelancer that owner's post choise
    public Guid? FreelancerId { get; set; }
    
     public User? Freelancer { get; set; }
}