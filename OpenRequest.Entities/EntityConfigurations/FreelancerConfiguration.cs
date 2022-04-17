using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Entities.EntityConfigurations;

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