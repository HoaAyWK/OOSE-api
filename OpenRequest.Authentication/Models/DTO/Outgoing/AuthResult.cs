namespace OpenRequest.Authentication.Models.DTO.Outgoing;

public class AuthResult
{
    public bool Success { get; set; }
    public List<string>? Errors { get; set; }
}