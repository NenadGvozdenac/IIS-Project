using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.BuildingBlocks.Infratructure.Database;

namespace travel_service.src.Travels.API.Startup;

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
        
        services.AddDbContext<TravelDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}