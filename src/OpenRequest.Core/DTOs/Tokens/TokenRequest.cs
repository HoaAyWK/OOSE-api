using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Tokens;

public class TokensRequest
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public string RefreshToken { get; set; } = null!;
}