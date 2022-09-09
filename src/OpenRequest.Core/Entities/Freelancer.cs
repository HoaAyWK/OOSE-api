namespace OpenRequest.Core.Entities;

public class Freelancer : User
{
    public List<Post>? Posts { get; set; }
    
    public List<FreelancerRequest>? FreelancerRequests { get; set; }
}