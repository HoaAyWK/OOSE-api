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
                dest => dest.Address,
                opt => opt.MapFrom(src => src.Address)
            )
            .ForMember(
                dest => dest.Country,
                opt => opt.MapFrom(src => src.Country)
            );
    }
}