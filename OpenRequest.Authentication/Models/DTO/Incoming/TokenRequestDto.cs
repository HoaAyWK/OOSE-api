using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Authentication.Models.DTO.Incoming;

public class TokenRequestDto
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public string RefreshToken { get; set; } = null!;
}