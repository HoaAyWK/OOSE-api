using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Authentication.Models.DTO.Incoming;

public class CustomerRegisrationDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;
    

    public string? Phone { get; set; }

    [Required]
    public string Password { get; set; } = null!;


    public string? Country { get; set; } 


    public string? Address { get; set; }

}