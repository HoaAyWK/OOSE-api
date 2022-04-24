using AutoMapper;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Api.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CustomerRegisrationDto, Customer>()
            .ForMember(
                dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => Convert.ToDateTime(src.DateOfBirth))
            );
    }
}