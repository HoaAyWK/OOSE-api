using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRequest.Core.Entities;

namespace SioayFreelance.Entities.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.AssignedPosts)
            .WithOne(p => p.Freelancer)
            .HasForeignKey(p => p.FreelancerId);
    }
}