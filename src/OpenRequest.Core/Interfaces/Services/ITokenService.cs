using System.Security.Claims;
using OpenRequest.Core.Dtos.Auth;
using OpenRequest.Core.Dtos.Tokens;

namespace OpenRequest.Core.Interfaces.Services;

public interface ITokenService
{
    Task<TokenDto> GenerateJwtTokenAsync(string email, string userId, List<Claim> claims);
    Task<AuthResult> RefreshTokenAsync(TokensRequest request);

    List<string>? GetRolesFromToken(string token);

    string GetUserId(string token);
}