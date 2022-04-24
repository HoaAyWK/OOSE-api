using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(FreelancerConfiguration))]
public class Freelancer : User
{
    public string JobTitle { get; set; } = null!;
    public List<Post>? Posts { get; set; }
}