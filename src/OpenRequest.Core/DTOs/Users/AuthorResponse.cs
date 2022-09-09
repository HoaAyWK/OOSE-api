namespace OpenRequest.Core.Dtos.Users;

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}