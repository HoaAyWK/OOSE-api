using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(CustomerConfigurations))]
public class Customer : User 
{
    public List<Post>? Posts { get; set; }
}