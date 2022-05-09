using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;


[EntityTypeConfiguration(typeof(PostRequestConfiguration))]
public class PostRequest 
{
    public Guid FreelancerId { get; set; }
    public Freelancer? Freelancer { get; set; }
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public int Status { get; set; } = 0;
}