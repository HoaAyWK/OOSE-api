using Microsoft.EntityFrameworkCore;
using OpenRequest.Entities.EntityConfigurations;

namespace OpenRequest.Entities.DbSets;

[EntityTypeConfiguration(typeof(PostCategoryConfiguration))]
public class PostCategory 
{
    public Guid PostId { get; set; }
    public Post? Post { get; set; } 

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}