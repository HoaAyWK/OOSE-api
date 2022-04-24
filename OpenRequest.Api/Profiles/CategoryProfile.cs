using AutoMapper;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Api.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryDto, Category>();
    }
}