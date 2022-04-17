using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(PostConfiguration))]
public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Status { get; set; } = 1;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public double Price { get; set; } 
    public double Duration { get; set; }

    public Guid AuthorId { get; set; }
    public Customer Author { get; set; } = null!;

    public List<PostCategory>? PostCategories { get; set; }

    // List of Freelancers that wanna to do the job
    public List<Freelancer>? Freelancers { get; set; }

    // Freelancer that owner's post choise
    public Guid? FreelancerId { get; set; }
    public Freelancer? Freelancer { get; set; }
}