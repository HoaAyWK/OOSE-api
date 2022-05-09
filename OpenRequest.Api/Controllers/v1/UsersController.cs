using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Configuration.Messages;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DbSets.Incoming;
using OpenRequest.Entities.DTO.Generic;
using OpenRequest.Entities.DTO.Outgoing;

namespace OpenRequest.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper) 
        : base(unitOfWork, userManager, mapper)
    {
    }

    [HttpGet]
    [Route("GetLoggedInUserInfo")]
    public async Task<IActionResult> GetLoggedInUserInfo() {
        var result = new Result<UserResponseDto>();
        var loggedInUser = await _userManger.GetUserAsync(HttpContext.User);
        if (loggedInUser == null) 
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Users.NotFound);
            return NotFound(result);
        }
        var roles = await _userManger.GetRolesAsync(loggedInUser);
        var identityId = new Guid(loggedInUser.Id);
        var user = await _unitOfWork.Users.GetByIdentityId(identityId);
        if (user == null) 
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Users.NotFound);
            return NotFound(result);
        }

        var mappedUser = _mapper.Map<UserResponseDto>(user);
        mappedUser.Roles = roles.ToList();
        
        result.Content = mappedUser;
        return Ok(result);
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = new Result<List<User>>();
        var users = await _unitOfWork.Users.All();
        result.Content = users.ToList();
        return Ok(result);
    }

    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = new Result<User>();
        var user = await _unitOfWork.Users.GetById(id);
        if (user == null)
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Users.NotFound);
            return NotFound(result);
        }
        result.Content = user;
        return Ok(result);
    }

    [HttpPut]
    [Route("UpdateAvatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UserAvatarDto userAvatarDto)
    {
        var result = new Result<string>();
        var userId = new Guid(userAvatarDto.UserId);
        var updateAvatar= await _unitOfWork.Users.UpdateAvatar(userId, userAvatarDto.FilePath);
        if (updateAvatar) 
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Updated User Avatar";
            return Ok(result);
        }
        
        result.Error = PopulateError(400,
            ErrorMessages.Type.BadRequest,
            ErrorMessages.Users.UnableToProcess
        );
        return BadRequest(result);
    }

    [HttpPut]
    [Route("UpdateBackground")]
    public async Task<IActionResult> UpdateBackground([FromBody] UserAvatarDto userAvatarDto)
    {
        var result = new Result<string>();
        var userId = new Guid(userAvatarDto.UserId);
        var updateAvatar= await _unitOfWork.Users.UpdateBackground(userId, userAvatarDto.FilePath);
        if (updateAvatar) 
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Updated User Background";
            return Ok(result);
        }
        
        result.Error = PopulateError(400,
            ErrorMessages.Type.BadRequest,
            ErrorMessages.Users.UnableToProcess
        );
        return BadRequest(result);
    }

    [HttpPut]
    [Route("UpdateUserInfo")]
    public async Task<IActionResult> UpdateUserInfo(Guid id, [FromBody] UpdateUserInfoDto updateUserInfoDto)
    {
        var result = new Result<UserResponseDto>();
        if (ModelState.IsValid)
        {
            var updated = await _unitOfWork.Users.UpdateUserInfo(id, updateUserInfoDto);
            if (updated) 
            {
                await _unitOfWork.CompleteAsync();
                var user = await _unitOfWork.Users.GetById(id);
                var mappedUser = _mapper.Map<UserResponseDto>(user);
                result.Content = mappedUser;
                return Ok(result);
            }

            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Users.UnableToProcess
            );
            return BadRequest(result);
        }
        else 
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Users.UnableToProcess
            );
            return BadRequest(result);
        }
    }
}

