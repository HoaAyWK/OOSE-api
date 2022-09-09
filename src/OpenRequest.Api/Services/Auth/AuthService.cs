using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OpenRequest.Core.Dtos.Auth;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Interfaces.Services;

namespace OpenRequest.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager,
        IMapper mapper,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public Task<AuthResult> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var userExist = await _userManager.FindByEmailAsync(request.Email);
        if (userExist == null) 
        {
            return new AuthResult
            {
                Success = false,
                Errors = new List<string>{ "Invalid authentication request." }
            };
        }
        
        var validPassword = await _userManager.CheckPasswordAsync(userExist, request.Password);
        if (validPassword) 
        {
            var identityId = new Guid(userExist.Id);
            var appUser = await _unitOfWork.Users.GetUserByIdentityId(identityId);
            if (appUser == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>{ "Invalid authentication request." }
                };
            }

            var userRoles = await _userManager.GetRolesAsync(userExist);
            var claims = new List<Claim>
            {
                new Claim("Id", appUser.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Email),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var token = await _tokenService.GenerateJwtTokenAsync(appUser.Email, appUser.Id.ToString(), claims);

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
                Errors = new List<string>{ "Invalid authentication request." }
            };
        }
    }

    public async Task<RegisterResponse> RegisterAsync(IRegisterRequest request)
    {
        var result = new RegisterResponse();
        var userExist = await _userManager.FindByEmailAsync(request.Email);
        if (userExist != null)
        {
            result.Errors = new List<string> { "Email already in use." };
            return result;
        }

        var user = new IdentityUser()
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = false
        };

        var create = await _userManager.CreateAsync(user, request.Password);
        if (!create.Succeeded)
        {
            result.Errors = create.Errors.Select(e => e.Description).ToList();
            return result;
        } 

        foreach (var role in request.Roles)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        var appUser = _mapper.Map<User>(request);
        appUser.IdentityId = new Guid(user.Id);
        var addedUser = await _unitOfWork.Users.Add(appUser);
        if (!addedUser)
        {
            await _userManager.DeleteAsync(user);
            result.Errors = new List<string> { "Can not create account." };
            return result;
        }

        await _unitOfWork.CompleteAsync();

        var claims = new List<Claim>
        {
            new Claim("Id", appUser.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Email),
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in request.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = await _tokenService.GenerateJwtTokenAsync(appUser.Email, appUser.Id.ToString(), claims);
        result.Token = token.JwtToken;
        result.RefreshToken = token.RefreshToken;

        return result;
    }
}