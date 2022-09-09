using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Infrastructure.Data;
using OpenRequest.Infrastructure.Services;

namespace OpenRequest.Api.Configurations;

public static class ConfigureInfrastructureServices
{
    public static void AddInFrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var defaultConnectionString = string.Empty;

        if (environment.EnvironmentName == "Development") 
        {
            defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
        else
        {
            // Use connection string provided at runtime by Heroku.
            var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            connectionUrl = connectionUrl!.Replace("postgres://", string.Empty);
            var userPassSide = connectionUrl.Split("@")[0];
            var hostSide = connectionUrl.Split("@")[1];

            var user = userPassSide.Split(":")[0];
            var password = userPassSide.Split(":")[1];
            var host = hostSide.Split("/")[0];
            var database = hostSide.Split("/")[1].Split("?")[0];

            defaultConnectionString = $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }

        services.AddDbContext<AppDbContext>(options => 
        {
            options.UseSqlServer(defaultConnectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });

        services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<ITokenService, TokenService>();

        var serviceProvider = services.BuildServiceProvider();
        try
        {
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
        catch
        {
        }
    }
}