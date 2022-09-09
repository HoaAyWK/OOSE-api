using OpenRequest.Core.Dtos.Auth;

namespace OpenRequest.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<RegisterResponse> RegisterAsync(IRegisterRequest request);
    Task<AuthResult> ConfirmEmailAsync(ConfirmEmailRequest request);
}