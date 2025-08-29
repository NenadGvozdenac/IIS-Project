using Microsoft.EntityFrameworkCore;
using match_service.src.Matches.BuildingBlocks.Infratructure.Database;

namespace match_service.src.Matches.API.Startup;

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
        
        services.AddDbContext<MatchDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}