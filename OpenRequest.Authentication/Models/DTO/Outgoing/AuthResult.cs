namespace OpenRequest.Authentication.Models.DTO.Outgoing;

public class AuthResult
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public List<string>? Errors { get; set; } 
    public string? CreatedDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DateOfBirth { get; set; }
     public string? Address { get; set; }
    public string? Country { get; set; }
    public string? FeaturedAvatar { get; set; }
     public string? FeaturedBackground { get; set; }
}