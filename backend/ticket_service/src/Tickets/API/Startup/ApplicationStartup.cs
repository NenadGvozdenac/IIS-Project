using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ticket_service.src.Tickets.Core.Infrastructure;
using ticket_service.src.Tickets.Core.Infrastructure.Services;
using ticket_service.src.Tickets.Core.Infrastructure.Repositories.Relational;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Infrastructure.Database;

namespace ticket_service.src.Tickets.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabases(services, configuration);
        SetupRepositories(services);
        SetupServices(services);
        SetupBackgroundServices(services);
        SetupMediatR(services);

        return services;
    }

    private static void SetupRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<ICreditCardRepository, CreditCardRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IPurchaseOfferRepository, PurchaseOfferRepository>();
        services.AddScoped<ISeasonTicketRepository, SeasonTicketRepository>();
        services.AddScoped<IIndividualTicketRepository, IndividualTicketRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<ITicketPriceParameterRepository, TicketPriceParameterRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        
        // Graph repositories
        services.AddScoped<IGraphSeatRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.SeatRepository>();
        services.AddScoped<IGraphMatchRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.MatchRepository>();
        services.AddScoped<IGraphCustomerRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.CustomerRepository>();
        services.AddScoped<IGraphIndividualTicketRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.IndividualTicketRepository>();
        
        // Graph relationship repositories
        services.AddScoped<IGraphBoughtRelationshipRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.BoughtRelationshipRepository>();
        services.AddScoped<IGraphIsForSeatRelationshipRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.IsForSeatRelationshipRepository>();
        services.AddScoped<IGraphIsForMatchRelationshipRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.IsForMatchRelationshipRepository>();
        
        // Graph complex query repository
        services.AddScoped<IGraphComplexQueryRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.ComplexQueryRepository>();
        
        // Graph reports repository
        services.AddScoped<IGraphReportsRepository, ticket_service.src.Tickets.Core.Infrastructure.Repositories.Graph.ReportsRepository>();
    }

    private static void SetupServices(IServiceCollection services)
    {
        services.AddScoped<ICreditCardEncryptionService, CreditCardEncryptionService>();
        services.AddScoped<ITicketPriceCalculationService, TicketPriceCalculationService>();
        services.AddScoped<INeo4jSeedingService, Neo4jSeedingService>();
    }

    private static void SetupBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<MatchFinishedService>();
        services.AddHostedService<Neo4jSeedingHostedService>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }

    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";

        services.AddDbContext<TicketDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IGraphDatabaseContext, Neo4jDatabaseContext>();
    }
}