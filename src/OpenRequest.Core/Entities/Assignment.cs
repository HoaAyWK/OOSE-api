namespace OpenRequest.Core.Entities;

public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Status { get; set; } = 1;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; }

    public Guid PostId { get; set; }

    public Post? Post { get; set; }

    public DateTime EndDate { get; set; }
    
    public string? FilePath { get; set; } 
}