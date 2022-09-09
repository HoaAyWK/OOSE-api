namespace OpenRequest.Core.Dtos.Auth;

public class AdminRegisterRequest : IRegisterRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string DateOfBirth { get; set; } = null!;

    public string? JobTitle { get; set; } = null;

    public string? CompanyName { get; set; } = null;

    public string Address { get; set; } = null!;

    public string? BaseUrl { get; set; }
    
    public List<string> Roles { get; set; } = new List<string> { "Admin" };
}