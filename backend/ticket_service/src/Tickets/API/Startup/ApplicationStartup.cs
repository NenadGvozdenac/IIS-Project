using MediatR;
using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.BuildingBlocks.Infratructure.Database;
using ticket_service.src.Tickets.Core.Application.Features.Users.GetUserById;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Infrastructure.Repositories;

namespace ticket_service.src.Tickets.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);

        SetupMediatR(services);

        SetupRepositories(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        // Register Specific Repositories
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        // Register MediatR for version 11.x
        services.AddMediatR(typeof(GetUserByIdHandler).Assembly);
    }

    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<TicketDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}