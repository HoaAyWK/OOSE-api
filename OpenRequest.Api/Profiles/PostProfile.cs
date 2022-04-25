using AutoMapper;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Incoming;

namespace OpenRequest.Api.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostRequestDto, Post>();
    }
}