using AutoMapper;
using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Dtos.Errors;
using OpenRequest.Core.Dtos.Users;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Core.Interfaces.UoW;

namespace OpenRequest.Core.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uniOfWork;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService)
    {
        _uniOfWork = unitOfWork;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersAsync(string token)
    {
        var result = new Result<IEnumerable<UserResponseDto>>();
        var roles = _tokenService.GetRolesFromToken(token);

        if (roles == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };

            return result;
        }

        if (!roles.Contains("Admin"))
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };

            return result;
        }

        var users = await _uniOfWork.Users.All();
        var userDtos = _mapper.Map<IEnumerable<User>, IEnumerable<UserResponseDto>>(users);
        result.Content = userDtos;

        return result;
    }

    public async Task<Result<UserResponseDto>> GetUserByIdAsync(string token, Guid id)
    {
        var result = new Result<UserResponseDto>();
        var roles = _tokenService.GetRolesFromToken(token);
        
        if (roles == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };

            return result;
        }

        if (!roles.Contains("Admin"))
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };

            return result;
        }

        var user = await _uniOfWork.Users.GetById(id);
        var userDto = _mapper.Map<UserResponseDto>(user);
        result.Content = userDto;

        return result;
    }

    public async Task<Result<UserResponseDto>> GetUserInfoAsync(string token)
    {
        var result = new Result<UserResponseDto>();
        var userId = _tokenService.GetUserId(token);
        var user = await _uniOfWork.Users.GetById(new Guid(userId));

        if (user == null)
        {
            result.Error = new Error
            {
                Code = 404,
                Message = "User does not exist."
            };
        }

        var userDto = _mapper.Map<UserResponseDto>(user);
        result.Content = userDto;

        return result;
    }

    public async Task<Result<UserResponseDto>> UpdateAvatarAsync(string token, UserAvatarDto userAvatar)
    {
        var result = new Result<UserResponseDto>();
        var userId = _tokenService.GetUserId(token);
        var updated = await _uniOfWork.Users.UpdateAvatar(new Guid(userId), userAvatar.FilePath);

        if (!updated)
        {
            result.Error = new Error
            {
                Code = 404,
                Message = "User does not exist"
            };

            return result;
        }

        await _uniOfWork.CompleteAsync();
        var updatedUser = await _uniOfWork.Users.GetById(new Guid(userId));
        var userDto = _mapper.Map<UserResponseDto>(updatedUser);
        result.Content = userDto;
        
        return result;
    }

    public async Task<Result<UserResponseDto>> UpdateBackgroundAsync(string token, UserBackgroundDto userBackground)
    {
        var result = new Result<UserResponseDto>();
        var userId = _tokenService.GetUserId(token);
        var updated = await _uniOfWork.Users.UpdateBackground(new Guid(userId), userBackground.FilePath);

        if (!updated)
        {
            result.Error = new Error
            {
                Code = 404,
                Message = "User does not exist"
            };

            return result;
        }

        await _uniOfWork.CompleteAsync();
        var updatedUser = await _uniOfWork.Users.GetById(new Guid(userId));
        var userDto = _mapper.Map<UserResponseDto>(updatedUser);
        result.Content = userDto;
        
        return result;
    }

    public async Task<Result<UserResponseDto>> UpdateUserInforAsyncAsync(string token, UpdateUserInfoDto updatedUserInfo)
    {
        var result = new Result<UserResponseDto>();
        var userId = _tokenService.GetUserId(token);
        var userMapped = _mapper.Map<User>(updatedUserInfo);
        var updated = await _uniOfWork.Users.UpdateUserInfo(new Guid(userId), userMapped);

        if (!updated)
        {
            result.Error = new Error
            {
                Code = 404,
                Message = "User does not exist"
            };

            return result;
        }

        await _uniOfWork.CompleteAsync();
        var updatedUser = await _uniOfWork.Users.GetById(new Guid(userId));
        var userDto = _mapper.Map<UserResponseDto>(updatedUser);
        result.Content = userDto;

        return result;
    }
}