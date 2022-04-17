using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Authentication.Models.DTO.Outgoing;
using OpenRequest.Entities.DbSets;
using OpenRequest.Authentication.Models.DTO.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using OpenRequest.Authentication.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace OpenRequest.Api.Controllers.v1;

public class AccountsController : BaseController
{
    private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AccountsController(IUnitOfWork unitOfWork, 
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> optionsMonitor,
        TokenValidationParameters tokenValidationParameters) 
        : base(unitOfWork, userManager)
    {
        _jwtConfig = optionsMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationRequestDto)
    {
        if (ModelState.IsValid) 
        {
            var userExist = await _userManger.FindByEmailAsync(registrationRequestDto.Email);

            if (userExist != null)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>{ "Email already in use." }
                });
            }

            var user = new IdentityUser()
            {
                Email = registrationRequestDto.Email,
                UserName = registrationRequestDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManger.CreateAsync(user, registrationRequestDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            var newUser = new User();
            newUser.IdentityId = new Guid(user.Id);
            newUser.Status = 1;
            newUser.Email = registrationRequestDto.Email;
            newUser.FirstName = registrationRequestDto.FirstName;
            newUser.LastName = registrationRequestDto.LastName;
            newUser.DateOfBirth =  DateTime.Parse(registrationRequestDto.DateOfBirth);
            newUser.Address = registrationRequestDto.Address;
            newUser.Gender = registrationRequestDto.Gender;
            newUser.Country = "";
            newUser.Phone = "";

            await _unitOfWork.Users.Add(newUser);
            await _unitOfWork.CompleteAsync();

            var token = await GenerateJwtToken(user);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken
            });
        }
        else
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>{ "Invalid payload." }
            });
        }

    }


    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequestDto)
    {
        if (ModelState.IsValid)
        {
            var userExist = await _userManger.FindByEmailAsync(loginRequestDto.Email);

            if (userExist == null)
            {
                return BadRequest(new UserLoginResponseDto
                {
                    Success = false,
                    Errors = new List<string>{ "Invalid authentication request." }
                });
            }

            var validPassword = await _userManger.CheckPasswordAsync(userExist, loginRequestDto.Password);

            if (validPassword)
            {
                var token = await GenerateJwtToken(userExist);

                return Ok(new UserLoginResponseDto
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
                });
            }
            else
            {
                return BadRequest(new UserLoginResponseDto
                {
                    Success = false,
                    Errors = new List<string>{ "Invalid authentication request." }
                });
            }
        }
        else
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>{ "Invalid payload." }
            });
        }
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
    {
        if (ModelState.IsValid)
        {
            var result = await VerifyToken(tokenRequestDto);
            if (result == null)
            {
                return BadRequest(new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Token validation failed." }
                });
            }

            return Ok(result);
        }
        else
        {
            return BadRequest(new AuthResult
            {
                Success = false,
                Errors = new List<string>{ "Invalid payload." }
            });
        }
    }

    private async Task<TokenData> GenerateJwtToken(IdentityUser user)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDesciptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = jwtHandler.CreateToken(tokenDesciptor);
        var jwtToken = jwtHandler.WriteToken(token);
        var refreshToken =  new RefreshToken
        {
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            UserId = user.Id,
            IsUsed = false,
            IsRevoked = false,
            JwtId = token.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();

        var tokenData = new TokenData 
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken.Token
        };
        
        return tokenData;
    }

    private string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }

    private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(tokenRequestDto.Token,
                _tokenValidationParameters, out var validatedToken);            
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase);
                
                if (!result)
                {
                    return null;
                }
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
                .GetByRefreshToken(tokenRequestDto.RefreshToken);          
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

            refreshTokenExist.IsUsed = true;      
            var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);
            if (updateResult)
            {
                await _unitOfWork.CompleteAsync();
                var user = await _userManger.FindByIdAsync(refreshTokenExist.UserId);
                if (user == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string>{ "Error processing request." }
                    };
                }

                var token = await GenerateJwtToken(user);
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