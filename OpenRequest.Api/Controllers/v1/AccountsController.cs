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
using AutoMapper;

namespace OpenRequest.Api.Controllers.v1;

public class AccountsController : BaseController
{
    private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AccountsController(IUnitOfWork unitOfWork, 
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> optionsMonitor,
        TokenValidationParameters tokenValidationParameters,
        IMapper mapper) 
        : base(unitOfWork, userManager, mapper)
    {
        _jwtConfig = optionsMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost]
    [Route("CustomerRegister")]
    public async Task<IActionResult> CustomerRegister([FromBody] CustomerRegisrationDto customerRegisrationDto)
    {
        if (ModelState.IsValid) 
        {
            var userExist = await _userManger.FindByEmailAsync(customerRegisrationDto.Email);

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
                Email = customerRegisrationDto.Email,
                UserName = customerRegisrationDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManger.CreateAsync(user, customerRegisrationDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            var addedRole = await _userManger.AddToRoleAsync(user, "Customer");
            if (!addedRole.Succeeded)
            {
                await _userManger.DeleteAsync(user);
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = addedRole.Errors.Select(e => e.Description).ToList()
                });
            }

            var customer = _mapper.Map<Customer>(customerRegisrationDto);
            customer.IdentityId = new Guid(user.Id);

            var addedCustomer = await _unitOfWork.Customers.Add(customer);
            if (!addedCustomer)
            {
                await _userManger.DeleteAsync(user);
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>{ "Can not add customer." }
                });
            }
            await _unitOfWork.CompleteAsync();

            var token = await GenerateJwtToken(user);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken,
                UserId = customer.Id.ToString(),
                UserAvatar = customer.FeaturedAvatar,
                Roles = new List<string>{ "Customer" },
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Country = customer.Country,
                DateOfBirth = customer.DateOfBirth.ToString("MM/dd/yyyy"),
                CreatedDate = customer.CreatedDate.ToString("MM/dd/yyyy"),
            });
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

    [HttpPost]
    [Route("FreelancerRegister")]
    public async Task<IActionResult> FreelancerRegister([FromBody] FreelancerRegistrationDto freelancerRegistrationDto)
    {
        if (ModelState.IsValid) 
        {
            var userExist = await _userManger.FindByEmailAsync(freelancerRegistrationDto.Email);

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
                Email = freelancerRegistrationDto.Email,
                UserName = freelancerRegistrationDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManger.CreateAsync(user, freelancerRegistrationDto.Password);     
            if (!result.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            var addedRole = await _userManger.AddToRoleAsync(user, "Freelancer");
            if (!addedRole.Succeeded)
            {
                await _userManger.DeleteAsync(user);
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = addedRole.Errors.Select(e => e.Description).ToList()
                });
            }

            var freelancer = _mapper.Map<Freelancer>(freelancerRegistrationDto);
            freelancer.IdentityId = new Guid(user.Id);

            var addedFreelancer = await _unitOfWork.Freelancers.Add(freelancer);
            if (!addedFreelancer)
            {
                await _userManger.DeleteAsync(user);
                return BadRequest(new UserRegistrationResponseDto{
                    Success = false,
                    Errors = new List<string>{ "Can not add freelancer." }
                });
            }
            await _unitOfWork.CompleteAsync();
            
            var token = await GenerateJwtToken(user);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken,
                UserId = freelancer.Id.ToString(),
                UserAvatar = freelancer.FeaturedAvatar,
                Roles = new List<string>{ "Freelancer" },
                FirstName = freelancer.FirstName,
                LastName = freelancer.LastName,
                Phone = freelancer.Phone,
                Email = freelancer.Email,
                Address = freelancer.Address,
                Country = freelancer.Country,
                DateOfBirth = freelancer.DateOfBirth.ToString("MM/dd/yyyy"),
                CreatedDate = freelancer.CreatedDate.ToString("MM/dd/yyyy"),
            });
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
                var IdentityId = new Guid(userExist.Id);
                var user = await _unitOfWork.Users.GetByIdentityId(IdentityId);
                var roles = await _userManger.GetRolesAsync(userExist);
                if (user == null) 
                {
                    if (roles.Contains("Admin")) 
                    {
                        var orUser = new Admin()
                        {
                            Email = userExist.Email,
                            FirstName = "Admin",
                            LastName = "Admin",
                            Phone="99999999999",
                            DateOfBirth = DateTime.UtcNow,
                            IdentityId = IdentityId,
                        };

                        var addedAdmin = await _unitOfWork.Users.Add(orUser);
                        if (addedAdmin)
                        {
                            await _unitOfWork.CompleteAsync();
                            user = orUser;
                        }
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
                var token = await GenerateJwtToken(userExist);

                
                return Ok(new UserLoginResponseDto
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken,
                    UserId = user.Id.ToString(),
                    Roles = roles.ToList(),
                    UserAvatar = user.FeaturedAvatar,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = user.Address,
                    Country = user.Country,
                    DateOfBirth = user.DateOfBirth.ToString("MM/dd/yyyy"),
                    CreatedDate = user.CreatedDate.ToString("MM/dd/yyyy"),
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
            _tokenValidationParameters.ValidateLifetime = false;
            var principal = tokenHandler.ValidateToken(tokenRequestDto.Token,
                _tokenValidationParameters, out var validatedToken); 
            _tokenValidationParameters.ValidateLifetime = true;           
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                
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