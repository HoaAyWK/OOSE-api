using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Configuration.Messages;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Generic;

namespace OpenRequest.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper) 
        : base(unitOfWork, userManager, mapper)
    {
    }

    [HttpGet]
    [Route("GetLoggedInUserInfo")]
    public async Task<IActionResult> GetLoggedInUserInfo() {
        var result = new Result<User>();
        var loggedInUser = await _userManger.GetUserAsync(HttpContext.User);
        if (loggedInUser == null) 
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Users.NotFound);
            return NotFound(result);
        }

        var identityId = new Guid(loggedInUser.Id);
        var user = await _unitOfWork.Users.GetByIdentityId(identityId);
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
}