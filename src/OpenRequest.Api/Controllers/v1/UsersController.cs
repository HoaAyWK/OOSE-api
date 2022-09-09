using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Core.Dtos.Users;
using OpenRequest.Core.Interfaces.Services;

namespace OpenRequest.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) : base()
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("get-current-user")]
    public async Task<IActionResult> GetCurrentUserAsync() {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.GetUserInfoAsync(token);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.GetAllUsersAsync(token);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpGet]
    [Route("get-user")]
    public async Task<IActionResult> GetUserAsync(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.GetUserByIdAsync(token, id);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut]
    [Route("update-avatar")]
    public async Task<IActionResult> UpdateAvatarAsync([FromBody] UserAvatarDto userAvatarDto)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.UpdateAvatarAsync(token, userAvatarDto);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut]
    [Route("update-background")]
    public async Task<IActionResult> UpdateBackgroundAsync([FromBody] UserBackgroundDto userBackgroundDto)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.UpdateBackgroundAsync(token, userBackgroundDto);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut]
    [Route("update-user-info")]
    public async Task<IActionResult> UpdateUserInfoAsync(Guid id, [FromBody] UpdateUserInfoDto updateUserInfoDto)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _userService.UpdateUserInforAsyncAsync(token, updateUserInfoDto);

        if (!result.IsSuccess)
            return BadRequest(result);
        
        return Ok(result);
    }
}

