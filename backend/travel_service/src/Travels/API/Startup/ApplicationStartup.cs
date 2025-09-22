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
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ITravelInfoRepository, TravelInfoRepository>();
        services.AddScoped<IVisaRepository, VisaRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<ICompetitionRepository, CompetitionRepository>();
        services.AddScoped<INationalityRepository, NationalityRepository>();
        services.AddScoped<IAgenciesRepository, AgenciesRepository>();
        services.AddScoped<IRequestsRepository, RequestsRepository>();
        services.AddScoped<IOffersRepository, OffersRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
    }

    private static void SetupMediatR(IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        
        // Registruj current assembly
        var currentAssembly = typeof(ApplicationStartup).Assembly;
        services.AddMediatR(currentAssembly);
    }
    
    private static void SetupDatabases(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432";
        
        services.AddDbContext<TravelDbContext>(options =>
          options.UseNpgsql(connectionString));
    }
}