using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRequest.Core.Entities;

namespace OpenRequest.Infrastructure.Configurations;

public class FreelancerConfiguration : IEntityTypeConfiguration<Freelancer>
{
    public void Configure(EntityTypeBuilder<Freelancer> builder)
    {
        builder.HasMany(f => f.Posts)
            .WithOne(p => p.Freelancer)
            .HasForeignKey(p => p.FreelancerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}