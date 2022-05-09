namespace OpenRequest.Entities.DTO.Incoming;

using System.ComponentModel.DataAnnotations;

public class FinishAssignmentDto 
{
    [Required]
    public string PostId { get; set; } = null!;

    [Required]
    public string FilePath { get; set; } = null!;
}