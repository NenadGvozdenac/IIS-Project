using Microsoft.EntityFrameworkCore;
using MediatR;
using match_service.src.Matches.BuildingBlocks.Infratructure.Database;
using match_service.src.Matches.Core.Application.Features.Users.GetUserById;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure.Repositories;

namespace match_service.src.Matches.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupMediatR(services);

        return services;
    }

    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<MatchDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        // Register MediatR for version 11.x
        services.AddMediatR(typeof(GetUserByIdHandler).Assembly);
    }
}