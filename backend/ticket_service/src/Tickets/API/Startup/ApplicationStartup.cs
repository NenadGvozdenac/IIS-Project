using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Infrastructure.Repositories;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Infrastructure.Services;

namespace ticket_service.src.Tickets.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupServices(services);
        SetupMediatR(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<ICreditCardRepository, CreditCardRepository>();
    }

    private static void SetupServices(IServiceCollection services)
    {
        services.AddScoped<ICreditCardEncryptionService, CreditCardEncryptionService>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<TicketDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}