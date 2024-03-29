namespace OpenRequest.Core.Dtos.Users;

public class UserResponseDto
{
    public int Status { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? CreatedDate { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string? FeaturedAvatar { get; set; }

    public string? FeaturedBackground { get; set; }
    
    public List<string>? Roles { get; set; }
}