using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(CustomerConfigurations))]
public class Customer : User 
{
    public string CompanyName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public double Rate { get; set; }

    public List<Post>? Posts { get; set; }
}