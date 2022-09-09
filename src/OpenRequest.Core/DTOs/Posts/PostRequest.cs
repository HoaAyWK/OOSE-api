using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Posts;


public class PostRequest
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string FeaturedImage { get; set; } = null!;

    [Required]
    public double Price { get; set; }

    [Required]
    public double Duration { get; set; }

    [Required]
    public List<Guid> Categories { get; set; } = null!;
}