using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Authentication.Models.DTO.Incoming;

public class UserLoginRequestDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}