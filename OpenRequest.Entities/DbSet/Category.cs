namespace OpenRequest.Entities.DbSets;

public class Category 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public List<PostCategory>? PostCategories { get; set; }
}