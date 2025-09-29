using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Reflection;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure.Repositories;
using match_service.src.Matches.Core.Infrastructure;

namespace match_service.src.Matches.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupMediatR(services);
        services.ConfigureInfluxDB(configuration);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMatchTrackingRepository, MatchTrackingRepository>();
        services.AddScoped<ITeamMemberMatchRepository, TeamMemberMatchRepository>();
        services.AddScoped<IPersonalEventRepository, PersonalEventRepository>();
        services.AddScoped<ITeamEventRepository, TeamEventRepository>();
        services.AddScoped<IGeneralEventRepository, GeneralEventRepository>();
        services.AddScoped<IAutomaticRecommendationRepository, AutomaticRecommendationRepository>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<MatchDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}