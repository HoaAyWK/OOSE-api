namespace OpenRequest.Core.Dtos.Assignments;

public class AssignmentResponse
{
    public Guid Id { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? FilePath { get; set; } 
}