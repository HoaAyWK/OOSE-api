namespace OpenRequest.Core.Entities;
public class Customer : User 
{
    public List<Post>? Posts { get; set; }
}