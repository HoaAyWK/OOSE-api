using Microsoft.AspNetCore.Mvc;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Core.Dtos.Auth;
using OpenRequest.Core.Dtos.Tokens;

namespace OpenRequest.Api.Controllers.v1;

public class AccountsController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    public AccountsController(IAuthService authService, ITokenService tokenService) : base()
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("admin-register")]
    public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("customer-register")]
    public async Task<IActionResult> CustomerRegister([FromBody] CustomerRegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("freelancer-register")]
    public async Task<IActionResult> FreelancerRegister([FromBody] FreelancerRegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokensRequest request)
    {
        var result = await _tokenService.RefreshTokenAsync(request);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}