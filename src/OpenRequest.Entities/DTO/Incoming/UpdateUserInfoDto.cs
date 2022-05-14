using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Entities.DbSets.Incoming;

public class UpdateUserInfoDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;

    [Required]
    public string DateOfBirth { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;
}