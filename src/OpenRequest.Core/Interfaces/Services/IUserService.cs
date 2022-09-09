using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Dtos.Users;

namespace OpenRequest.Core.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserResponseDto>> GetUserInfoAsync(string token);

    Task<Result<IEnumerable<UserResponseDto>>> GetAllUsersAsync(string token);

    Task<Result<UserResponseDto>> GetUserByIdAsync(string token, Guid id);

    Task<Result<UserResponseDto>> UpdateAvatarAsync(string token, UserAvatarDto userAvatar);

    Task<Result<UserResponseDto>> UpdateBackgroundAsync(string token, UserBackgroundDto userBackground);

    Task<Result<UserResponseDto>> UpdateUserInforAsyncAsync(string token, UpdateUserInfoDto updatedUserInfo);
}