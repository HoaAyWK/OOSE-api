using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Authentication.Models.DTO.Generic;

public class TokenData
{
    [Required]
    public string JwtToken { get; set; } = null!;

    [Required]
    public string RefreshToken { get; set; } = null!;
}