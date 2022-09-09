using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenRequest.Core.Configurations;
using OpenRequest.Core.Dtos.Auth;
using OpenRequest.Core.Dtos.Tokens;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Interfaces.Services;

namespace OpenRequest.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenService(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<JwtConfig> optionsMonitor,
        TokenValidationParameters tokenValidationParameters,
        UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _jwtConfig = optionsMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
        _userManager = userManager;
    }

    public async Task<TokenDto> GenerateJwtTokenAsync(string email, string userId, List<Claim> claims)
    {       
        var jwtHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
        var tokenDesciptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };  

        var token = jwtHandler.CreateToken(tokenDesciptor);
        var jwtToken = jwtHandler.WriteToken(token);
        var refreshToken = new RefreshToken
        {
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            UserId = new Guid(userId),
            IsUsed = false,
            IsRevoked = false,
            JwtId = token.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(3)
        };

        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();

        var tokens = new TokenDto
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken.Token
        };

        return tokens;
    }

    public List<string>? GetRolesFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, 
                _tokenValidationParameters, out var validatedToken);
            if (validatedToken is JwtSecurityToken jwtSecurityToken) 
            {
                var result = jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, 
                    StringComparison.InvariantCultureIgnoreCase);

                if (!result)
                    return null!;
            }

            var claims = principal.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            var roles = new List<string>();

            foreach (var claim in claims)
            {
                roles.Add(claim.Value);
            }

            return roles;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public string GetUserId(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, 
                _tokenValidationParameters, out var validatedToken);
            if (validatedToken is JwtSecurityToken jwtSecurityToken) 
            {
                var result = jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, 
                    StringComparison.InvariantCultureIgnoreCase);

                if (!result)
                    return null!;
            }

            return principal.Claims.SingleOrDefault(x => x.Type == "Id").Value;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(TokensRequest request)
    {
        var result = await VerifyTokenAsync(request);
        if (result == null)
        {
            return new AuthResult
            {
                Success = false,
                Errors = new List<string>{ "Token validation failed." }
            };
        }
        return new AuthResult
        {
            Success = true,
            Token = result.Token,
            RefreshToken = result.RefreshToken
        };
    }


    private string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }

    private async Task<AuthResult?> VerifyTokenAsync(TokensRequest tokens)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try 
        {
            var principal = tokenHandler.ValidateToken(tokens.Token, 
                _tokenValidationParameters, out var validatedToken);
            if (validatedToken is JwtSecurityToken jwtSecurityToken) 
            {
                var result = jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, 
                    StringComparison.InvariantCultureIgnoreCase);

                if (!result)
                    return null!;
            }

            var utcExpiryDate = long.Parse(principal.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
            if (expiryDate > DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Jwt token has not expired." }
                };
            }

            var refreshTokenExist = await _unitOfWork.RefreshTokens
                .GetByRefreshToken(tokens.RefreshToken);
            if (refreshTokenExist == null) 
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Invalid refresh token." }
                };
            }

            if (refreshTokenExist.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Refresh token has expired, please login again." }
                };
            }

            if (refreshTokenExist.IsUsed)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Refesh token has been used, it cannot be reused." }
                };
            }

            if (refreshTokenExist.IsRevoked)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Refresh token has been revoked, it cannot be reused." }
                };
            }

            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (refreshTokenExist.JwtId != jti)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Refresh token reference does not match the jwt token." }
                };
            }
     
            var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);
            if (updateResult)
            {
                await _unitOfWork.CompleteAsync();
                var user = await _unitOfWork.Users.GetById(refreshTokenExist.UserId);
                if (user == null) 
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>{ "Error processing request." }
                    };
                }

                var claims = principal.Claims;              
                var token = await GenerateJwtTokenAsync(user.Email, refreshTokenExist.UserId.ToString(), claims.ToList());
                return new AuthResult
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
                };
            }
            else
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Error processing request." }
                };
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private DateTime UnixTimeStampToDateTime(long unixDate)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        dateTime.AddSeconds(unixDate).ToUniversalTime();

        return dateTime;
    }

}