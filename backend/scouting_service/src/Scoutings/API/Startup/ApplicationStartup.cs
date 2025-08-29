using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.BuildingBlocks.Infratructure.Database;

namespace scouting_service.src.Scoutings.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);

        return services;
    }

    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<ScoutingDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}