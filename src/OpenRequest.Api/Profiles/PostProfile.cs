using AutoMapper;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Incoming;
using OpenRequest.Entities.DTO.Outgoing;

namespace OpenRequest.Api.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostRequestDto, Post>();
    }
}