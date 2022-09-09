namespace OpenRequest.Core.Dtos.Auth;

public class RegisterResponse
{
    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    public string? Message { get; set; }

    public List<string>? Errors { get; set; } 
    
    public bool Success => Errors == null;
}