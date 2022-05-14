namespace OpenRequest.Authentication.Models.DTO.Outgoing;

public class UserRegistrationResponseDto : AuthResult
{
    public string? UserAvatar { get; set; }
    public List<string>? Roles { get; set; }
    public string? UserId { get; set; }
}