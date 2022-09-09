using OpenRequest.Api.Mapping;

namespace OpenRequest.Api.Configurations;

public static class ConfigureApiServices
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
    }
}