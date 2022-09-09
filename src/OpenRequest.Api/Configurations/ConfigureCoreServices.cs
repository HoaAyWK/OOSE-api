using OpenRequest.Api.Services.Auth;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Services;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Api.Configurations;

public static class ConfigureCoreServices
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IUserService, UserService>();
        
    }
}