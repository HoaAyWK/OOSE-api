using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(FreelancerConfiguration))]
public class Freelancer : User
{
    public List<Post>? Posts { get; set; }
    public List<PostRequest>? PostRequests { get; set; }
}