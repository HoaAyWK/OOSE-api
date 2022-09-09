using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRequest.Core.Entities;

namespace OpenRequest.Infrastructure.Configurations;

public class FreelancerRequestConfiguration : IEntityTypeConfiguration<FreelancerRequest>
{
    public void Configure(EntityTypeBuilder<FreelancerRequest> builder)
    {
        builder.HasKey(pr => new { pr.PostId, pr.FreelancerId });

        builder.HasOne(pr => pr.Post)
            .WithMany(p => p.FreelancerRequests)
            .HasForeignKey(pc=> pc.PostId);
        
        builder.HasOne(pr => pr.Freelancer)
            .WithMany(f => f.FreelancerRequests)
            .HasForeignKey(pc => pc.FreelancerId);
    }
}