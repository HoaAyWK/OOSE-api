using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Authentication.Models.DTO.Outgoing;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Api.Controllers.v1;

public class AccountsController : BaseController
{
    public AccountsController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) 
        : base(unitOfWork, userManager)
    {

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
            newUser.DateOfBirth = registrationRequestDto.DateOfBirth;
            newUser.Address = registrationRequestDto.Address;
            newUser.Gender = registrationRequestDto.Gender;
            newUser.Country = "";
            newUser.Phone = "";

            await _unitOfWork.Users.Add(newUser);
            await _unitOfWork.CompleteAsync();

            return Ok(new UserRegistrationResponseDto
            {
                Success = true
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
                return Ok(new UserLoginResponseDto
                {
                    Success = true
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
}