using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Authentication.Models.DTO.Incoming;

public class UserRegistrationRequestDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string Gender { get; set; } = null!;

    [Required]
    public string DateOfBirth { get; set; } = null!;
}