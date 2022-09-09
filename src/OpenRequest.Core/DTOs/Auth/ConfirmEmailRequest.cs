using System.ComponentModel.DataAnnotations;

namespace OpenRequest.Core.Dtos.Auth;

public class ConfirmEmailRequest
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Code { get; set; } = null!;
}