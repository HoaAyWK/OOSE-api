using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Entities.DTO.Incoming;

public class CategoryDto
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;
}