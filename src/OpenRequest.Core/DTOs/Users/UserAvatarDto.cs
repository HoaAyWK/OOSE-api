using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Users;

public class UserAvatarDto 
{
    [Required]
    public string FilePath { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
}