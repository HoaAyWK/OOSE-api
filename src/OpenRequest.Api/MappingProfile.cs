using AutoMapper;
using OpenRequest.Core.Dtos.Assignments;
using OpenRequest.Core.Dtos.Auth;
using OpenRequest.Core.Dtos.Categories;
using OpenRequest.Core.Dtos.Posts;
using OpenRequest.Core.Dtos.Users;
using OpenRequest.Core.Entities;

namespace OpenRequest.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Users
        CreateMap<CustomerRegisterRequest, User>();
        CreateMap<FreelancerRegisterRequest, User>();  
        CreateMap<AdminRegisterRequest, User>();
        CreateMap<User, UserResponseDto>();
        CreateMap<UpdateUserInfoDto, User>();

        CreateMap<User, AuthorResponse>();
        CreateMap<User, FreelancerResponse>();

        // Posts
        CreateMap<Post, PostResponse>();
        CreateMap<PostRequest, Post>();
        CreateMap<Post, PostDetailResponse>();
        CreateMap<Post, PostFullDetailResponse>();

        // PostCategory
        CreateMap<PostCategory, PostCategoryResponse>();

        // FreelancerRequest
        CreateMap<FreelancerRequest, FreelancerRequestResponse>();

        // Category
        CreateMap<CategoryRequest, Category>();
        CreateMap<Category, CategoryResponse>();

        // Assignment
        CreateMap<Assignment, AssignmentResponse>();
    }
}