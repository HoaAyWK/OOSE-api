using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Users;

public class UserBackgroundDto 
{
    [Required]
    public string FilePath { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
}