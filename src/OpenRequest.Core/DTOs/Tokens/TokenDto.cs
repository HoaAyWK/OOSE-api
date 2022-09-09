using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Tokens;

public class TokenDto
{
    [Required]
    public string JwtToken { get; set; } = null!;

    [Required]
    public string RefreshToken { get; set; } = null!;
}