using AutoMapper;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Outgoing;

namespace OpenRequest.Api.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponseDto>()
            .ForMember(
                dest => dest.CreatedDate,
                opt => opt.MapFrom(src => src.CreatedDate.ToString("MM/dd/yyyy"))
            );
    }
}