using Microsoft.EntityFrameworkCore;
using MediatR;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Infrastructure.Repositories;
using scouting_service.src.Scoutings.Core.Infrastructure;
using System.Reflection;

namespace scouting_service.src.Scoutings.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupMediatR(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<IMetricTypeRepository, MetricTypeRepository>();
        services.AddScoped<INationalityRepository, NationalityRepository>();
        services.AddScoped<IPhysicalMetricRepository, PhysicalMetricRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISessionMetricRepository, SessionMetricRepository>();
        services.AddScoped<ISessionStatusRepository, SessionStatusRepository>();
        services.AddScoped<ISessionTypeRepository, SessionTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<ScoutingDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}