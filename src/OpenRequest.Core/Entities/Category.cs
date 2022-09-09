namespace OpenRequest.Core.Entities;

public class Category 
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    
    public string? FeaturedImage { get; set; }

    public List<PostCategory>? PostCategories { get; set; }

    private Category()
    {
    }
    
    public Category(string name, string description, string image)
    {
        Name = name;
        Description = description;
        FeaturedImage = image;
    }
}