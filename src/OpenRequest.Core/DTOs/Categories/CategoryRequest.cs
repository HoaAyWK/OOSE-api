using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Categories;

public class CategoryRequest
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Image { get; set; } = null!;

    public string? Parent { get; set; }
}