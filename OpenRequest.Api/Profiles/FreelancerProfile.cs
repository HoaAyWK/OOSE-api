using AutoMapper;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.Api.Profiles;

public class FreelancerProfile : Profile
{
    public FreelancerProfile()
    {
        CreateMap<FreelancerRegistrationDto, Freelancer>()
        .ForMember(
                dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => Convert.ToDateTime(src.DateOfBirth))
        );;
    }
}