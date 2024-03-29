namespace OpenRequest.Core.Entities;

public class FreelancerRequest 
{
    public Guid FreelancerId { get; set; }

    public User? Freelancer { get; set; }

    public Guid PostId { get; set; }
    
    public Post? Post { get; set; }
}