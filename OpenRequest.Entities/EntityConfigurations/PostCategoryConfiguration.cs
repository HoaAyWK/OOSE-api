    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenRequest.Entities.DbSets;

    namespace OpenRequest.Entities.EntityConfigurations;

    public class PostCategoryConfiguration : IEntityTypeConfiguration<PostCategory>
    {
        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.HasKey(pc => new { pc.PostId, pc.CategoryId });

            builder.HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pc => pc.PostId);
            
            builder.HasOne(pc => pc.Category)
                .WithMany(pc => pc.PostCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }