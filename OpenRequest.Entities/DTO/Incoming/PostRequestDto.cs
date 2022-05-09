using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Entities.DTO.Incoming;

public class PostRequestDto
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    public string FeaturedImage { get; set; } = null!;

    [Required]
    public double Price { get; set; }

    [Required]
    public double Duration { get; set; } 

    [Required]
    public List<Guid> Categories { get; set; } = null!; 
}