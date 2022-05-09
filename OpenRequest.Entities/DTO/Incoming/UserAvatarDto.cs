using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Entities.DbSets.Incoming;

public class UserAvatarDto 
{
    [Required]
    public string FilePath { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
}