using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Entities.EntityConfigurations;

public class PostRequestConfiguration : IEntityTypeConfiguration<PostRequest>
{
    public void Configure(EntityTypeBuilder<PostRequest> builder)
    {
        builder.HasKey(pr => new { pr.PostId, pr.FreelancerId });

        builder.HasOne(pr => pr.Post)
            .WithMany(p => p.PostRequests)
            .HasForeignKey(pc=> pc.PostId);
        
        builder.HasOne(pr => pr.Freelancer)
            .WithMany(f => f.PostRequests)
            .HasForeignKey(pc => pc.FreelancerId);
    }
}