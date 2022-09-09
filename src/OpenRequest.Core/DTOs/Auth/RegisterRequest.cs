namespace OpenRequest.Core.Dtos.Auth;

public interface IRegisterRequest
{
    string Email { get; set; }
    string Password { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Phone { get; set; }
    string Address { get; set; }
    List<string> Roles { get; set; }
}