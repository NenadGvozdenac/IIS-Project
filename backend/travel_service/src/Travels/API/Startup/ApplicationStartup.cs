using Microsoft.EntityFrameworkCore;
using MediatR;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Infrastructure.Repositories;
using System.Reflection;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.API.Startup;

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
        
        services.AddDbContext<TravelDbContext>(options =>
          options.UseNpgsql(connectionString));
    }
}