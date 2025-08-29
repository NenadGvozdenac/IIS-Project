using Microsoft.EntityFrameworkCore;
using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Infratructure.Database;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Infrastructure.Repositories;
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